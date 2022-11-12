using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Azure;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;
using Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker;
using Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
    {
        loggingBuilder.AddConsole(consoleLoggerOptions => consoleLoggerOptions.TimestampFormat = "[HH:mm:ss]");
    })
    .ConfigureServices((hostContext, services) =>
    {
        NativeInjector.RegisterServices(services);

        var appSettings = new AppSettings();
        hostContext.Configuration.Bind("AppSettings", appSettings);
        services.AddSingleton(appSettings);

        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClient(configuration["AppSettings.ServiceBusSettings.PrimaryConnectionString"]);
        });

        services.Configure<TelemetryConfiguration>(
            (config) =>
            {
                config.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
            }
        );

        services.AddHostedService<Processor>();
    })
    .Build();

await host.RunAsync();
