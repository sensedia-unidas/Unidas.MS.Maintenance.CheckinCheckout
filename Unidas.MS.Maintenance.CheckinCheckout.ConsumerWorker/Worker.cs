using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;

namespace Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker
{
    public abstract class Worker : BackgroundService
    {
        protected readonly ILogger<Worker> _logger;
        protected readonly ICheckinCheckoutService _checkinCheckoutService;
        protected readonly AppSettings _appSettings;
        protected ServiceBusProcessor _processor;
        protected ServiceBusClient _serviceBusClient;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory factory)
        {
            _logger = logger;
            _checkinCheckoutService = factory.CreateScope().ServiceProvider.GetRequiredService<ICheckinCheckoutService>();
            _appSettings = factory.CreateScope().ServiceProvider.GetRequiredService<AppSettings>();

            _serviceBusClient = new ServiceBusClient(_appSettings.ServiceBusSettings.PrimaryConnectionString);
            _processor = _serviceBusClient.CreateProcessor(_appSettings.ServiceBusSettings.QueueName, new ServiceBusProcessorOptions());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _processor.ProcessMessageAsync += MessageHandler;
                _processor.ProcessErrorAsync += ErrorHandler;

                await _processor.StartProcessingAsync();
                _logger.LogInformation("Processando menssagens");

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                await _processor.CloseAsync(cancellationToken: stoppingToken);
            }
            finally
            {
                await _processor.DisposeAsync();
                await _serviceBusClient.DisposeAsync();
            }
        }

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                var body = args.Message.Body.ToString();
                var request = JsonConvert.DeserializeObject<ItemCheckinCheckoutRequestViewModel>(body);

                if (request == null)
                {
                    _logger.LogError("Não foi possível deserializar a menssagem: {0}", body);
                    return;
                }

                if(await ProcessMessage(request, args.CancellationToken))
                {
                    await args.CompleteMessageAsync(args.Message);
                }               
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError("Erro no processamento: {0}", args.Exception.Message.ToString());
            return Task.CompletedTask;
        }

        protected abstract Task<bool> ProcessMessage(ItemCheckinCheckoutRequestViewModel request, CancellationToken cancellationToken);
    }
}