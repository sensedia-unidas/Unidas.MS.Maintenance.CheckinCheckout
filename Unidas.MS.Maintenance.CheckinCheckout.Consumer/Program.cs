using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;
using Unidas.MS.Maintenance.CheckinCheckout.Consumer;
using Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                NativeInjector.RegisterServices(services);
                services.AddScoped<IServiceBusConsumer, ServiceBusConsumer>();

                var appSettings = new AppSettings();
                hostContext.Configuration.Bind("AppSettings", appSettings);
                services.AddSingleton(appSettings);

                services.Configure<TelemetryConfiguration>(
                    (config) =>
                    {
                        config.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
                    }
                );

                //var provider = services.BuildServiceProvider();
                //provider.GetRequiredService<Consumer>().ExecuteAsync().Wait();
            });

using var host = builder.Build();
//builder.Build();

using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;

IServiceBusConsumer consumer = provider.GetRequiredService<IServiceBusConsumer>();
await consumer.ExecuteAsync();
//ICheckinCheckoutService checkinCheckoutService = provider.GetRequiredService<ICheckinCheckoutService>();
//ILogger<Program> logger = provider.GetRequiredService<ILogger<Program>>();

//ServiceBusClient serviceBusClient = new ServiceBusClient(configuration["AppSettings:ServiceBusSettings:PrimaryConnectionString"]);
//ServiceBusProcessor processor = serviceBusClient.CreateProcessor(configuration["AppSettings:ServiceBusSettings:QueueName"], new ServiceBusProcessorOptions());

//try
//{
//    processor.ProcessMessageAsync += MessageHandler;
//    processor.ProcessErrorAsync += ErrorHandler;

//    await processor.StartProcessingAsync();

//    logger.LogInformation("Maintenance Checkin/Checkout - Processing messages");
//    Console.ReadKey();
//}
//finally
//{
//    await processor.DisposeAsync();
//    await serviceBusClient.DisposeAsync();
//}

//host.Run();

//async Task MessageHandler(ProcessMessageEventArgs args)
//{
//    var body = args.Message.Body.ToString();
//    var request = JsonConvert.DeserializeObject<ItemCheckinCheckoutRequestViewModel>(body);

//    logger.LogInformation("Maintenance Checkin/Checkout - Iniciando processamento o item", request);
//    var response = await checkinCheckoutService.Integrate(request);

//    if (!response.IsValid)
//    {
//        await args.AbandonMessageAsync(args.Message);
//        logger.LogInformation("Maintenance Checkin/Checkout - falha no processamento do item", request);
//        return;
//    }

//    logger.LogInformation("Maintenance Checkin/Checkout - Processamento realizado com sucesso", request);
//    await args.CompleteMessageAsync(args.Message);    
//}

//Task ErrorHandler(ProcessErrorEventArgs args)
//{
//    logger.LogError("Maintenance Checkin/Checkout - erro no processamento", args.Exception.ToString());
//    return Task.CompletedTask;
//}