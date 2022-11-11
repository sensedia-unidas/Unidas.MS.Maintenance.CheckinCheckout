using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;

namespace Unidas.MS.Maintenance.CheckinCheckout.Worker
{
    public class Worker //: BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICheckinCheckoutService _checkinCheckoutService;
        private readonly AppSettings _appSettings;

        public Worker(ILogger<Worker> logger,
            ICheckinCheckoutService checkinCheckoutService,
            AppSettings appSettings)
        {
            _logger = logger;
            _checkinCheckoutService = checkinCheckoutService;
            _appSettings = appSettings;

            ServiceBusClient serviceBusClient = new ServiceBusClient(configuration["AppSettings:ServiceBusSettings:PrimaryConnectionString"]);
            ServiceBusProcessor processor = serviceBusClient.CreateProcessor(configuration["AppSettings:ServiceBusSettings:QueueName"], new ServiceBusProcessorOptions());
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //        await Task.Delay(1000, stoppingToken);
        //    }
        //}

        public async Task ExecuteAsync()
        {
            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync();

                logger.LogInformation("Maintenance Checkin/Checkout - Processing messages");
                Console.ReadKey();
            }
            finally
            {
                await processor.DisposeAsync();
                await serviceBusClient.DisposeAsync();
            }
        }
    }
}