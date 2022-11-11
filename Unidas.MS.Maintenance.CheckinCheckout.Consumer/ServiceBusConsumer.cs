using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;

namespace Unidas.MS.Maintenance.CheckinCheckout.Consumer
{
    public class ServiceBusConsumer : IServiceBusConsumer
    {
        private readonly ILogger<ServiceBusConsumer> _logger;
        private readonly ICheckinCheckoutService _checkinCheckoutService;
        private readonly AppSettings _appSettings;
        private ServiceBusProcessor _processor;
        private ServiceBusClient _serviceBusClient;

        public ServiceBusConsumer(ILogger<ServiceBusConsumer> logger,
            ICheckinCheckoutService checkinCheckoutService,
            AppSettings appSettings)
        {
            _logger = logger;
            _checkinCheckoutService = checkinCheckoutService;
            _appSettings = appSettings;

            _serviceBusClient = new ServiceBusClient(_appSettings.ServiceBusSettings.PrimaryConnectionString);
            _processor = _serviceBusClient.CreateProcessor(_appSettings.ServiceBusSettings.QueueName, new ServiceBusProcessorOptions());
        }

        public async Task ExecuteAsync()
        {
            try
            {
                _processor.ProcessMessageAsync += MessageHandler;
                _processor.ProcessErrorAsync += ErrorHandler;

                await _processor.StartProcessingAsync();

                _logger.LogInformation("Maintenance Checkin/Checkout - Processing messages");
            }
            finally
            {
                await _processor.DisposeAsync();
                await _serviceBusClient.DisposeAsync();
            }
        }

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            var request = JsonConvert.DeserializeObject<ItemCheckinCheckoutRequestViewModel>(body);

            _logger.LogInformation("Maintenance Checkin/Checkout - Iniciando processamento o item", request);
            var response = await _checkinCheckoutService.Integrate(request);

            if (!response.IsValid)
            {
                await args.AbandonMessageAsync(args.Message);
                _logger.LogInformation("Maintenance Checkin/Checkout - falha no processamento do item", request);
                return;
            }

            _logger.LogInformation("Maintenance Checkin/Checkout - Processamento realizado com sucesso", request);
            await args.CompleteMessageAsync(args.Message);
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError("Maintenance Checkin/Checkout - erro no processamento", args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
