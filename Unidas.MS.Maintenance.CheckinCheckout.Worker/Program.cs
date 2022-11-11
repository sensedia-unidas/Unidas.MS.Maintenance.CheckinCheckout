using Microsoft.ApplicationInsights.Extensibility;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;
using Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC;
using Unidas.MS.Maintenance.CheckinCheckout.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        NativeInjector.RegisterServices(services);

        var appSettings = new AppSettings();
        hostContext.Configuration.Bind("AppSettings", appSettings);
        services.AddSingleton(appSettings);

        services.Configure<TelemetryConfiguration>(
            (config) =>
            {
                config.ConnectionString = hostContext.Configuration["ApplicationInsights:ConnectionString"];
            });

        //services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
