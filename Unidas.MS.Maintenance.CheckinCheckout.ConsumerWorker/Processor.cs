using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;

namespace Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker
{
    public class Processor : Worker
    {
        public Processor(ILogger<Worker> logger, IServiceScopeFactory factory) : base(logger, factory)
        {
        }

        protected override async Task ProcessMessage(ItemCheckinCheckoutRequestViewModel request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Maintenance Checkin/Checkout - Iniciando processamento o item", request);
            var response = await _checkinCheckoutService.Integrate(request);

            if (!response.IsValid)
            {
                _logger.LogInformation("Maintenance Checkin/Checkout - falha no processamento do item", request);
            }

            _logger.LogInformation("Maintenance Checkin/Checkout - Processamento realizado com sucesso", request);

            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);            
        }
    }
}
