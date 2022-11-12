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

        //public Worker(ILogger<Worker> logger) { _logger = logger; }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    await ExecuteServiceBusConsumerAsync();
        //    while (!stoppingToken.IsCancellationRequested)
        //    {                
        //        await Task.Delay(10000, stoppingToken);
        //    }  
            
        //}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _processor.ProcessMessageAsync += MessageHandler;
                _processor.ProcessErrorAsync += ErrorHandler;

                await _processor.StartProcessingAsync();
                _logger.LogInformation("Maintenance Checkin/Checkout - Processing messages");

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

                if (request != null)
                {
                    await ProcessMessage(request, args.CancellationToken);
                }
                else
                {
                    _logger.LogError("Não foi possível deserializar a menssagem: {body}", body);
                }

                //_logger.LogInformation("Maintenance Checkin/Checkout - Iniciando processamento o item", request);
                //var response = await _checkinCheckoutService.Integrate(request);

                //if (!response.IsValid)
                //{
                //    _logger.LogInformation("Maintenance Checkin/Checkout - falha no processamento do item", request);
                //}

                //_logger.LogInformation("Maintenance Checkin/Checkout - Processamento realizado com sucesso", request);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro", ex);
                throw;
            }
            
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError("Maintenance Checkin/Checkout - erro no processamento", args.Exception.ToString());
            return Task.CompletedTask;
        }

        protected abstract Task ProcessMessage(ItemCheckinCheckoutRequestViewModel request, CancellationToken cancellationToken);
    }
}