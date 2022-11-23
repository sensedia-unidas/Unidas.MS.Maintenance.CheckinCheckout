using Microsoft.Extensions.Logging;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;
using Unidas.MS.Maintenance.CheckinCheckout.Infra.Interfaces;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Services
{
    public class CheckinCheckoutService : ICheckinCheckoutService
    {
        private readonly IQueueConnectorApdater _queueConnectorAdapter;
        private readonly ILogger<CheckinCheckoutService> _logger;
        private readonly AppSettings _appSettings;
        public CheckinCheckoutService(AppSettings appSettings,
            ILogger<CheckinCheckoutService> logger,
            IQueueConnectorApdater queueConnectorAdapter)
        {
            _queueConnectorAdapter = queueConnectorAdapter;
            _logger = logger;
            _appSettings = appSettings;
        }
        public async Task Integrate(ItemCheckinCheckoutRequestViewModel request)
        {
            _logger.LogInformation("Iniciando integração", request);

            await _queueConnectorAdapter.SendMessage(_appSettings.ServiceBusSettings.PrimaryConnectionString, _appSettings.ServiceBusSettings.QueueName, request);            

            _logger.LogInformation("Finalizando integração", request);
        }
    }
}
