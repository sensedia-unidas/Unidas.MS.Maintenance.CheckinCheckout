using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging.ApplicationInsights;
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
    .ConfigureLogging((context, builder) =>
    {
        builder.AddConsole(consoleLoggerOptions => consoleLoggerOptions.TimestampFormat = "[HH:mm:ss]");

        // Providing a connection string is required if you're using the
        // standalone Microsoft.Extensions.Logging.ApplicationInsights package,
        // or when you need to capture logs during application startup, such as
        // in Program.cs or Startup.cs itself.
        builder.AddApplicationInsights(
            configureTelemetryConfiguration: (config) => config.ConnectionString = configuration["ApplicationInsights:ConnectionString"],
            configureApplicationInsightsLoggerOptions: (options) => options.TrackExceptionsAsExceptionTelemetry = false
        );

        // Capture all log-level entries from Program
        builder.AddFilter<ApplicationInsightsLoggerProvider>(
            typeof(Program).FullName, LogLevel.Trace);
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

        services.AddHostedService<Processor>();
    })
    .Build();


await host.RunAsync();
