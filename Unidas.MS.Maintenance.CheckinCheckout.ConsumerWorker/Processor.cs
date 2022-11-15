using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;

namespace Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker
{
    public class Processor : Worker
    {
        public Processor(ILogger<Worker> logger, IServiceScopeFactory factory) : base(logger, factory)
        {
        }

        protected override async Task<bool> ProcessMessage(ItemCheckinCheckoutRequestViewModel request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando processamento o item", request);
            var response = await _checkinCheckoutService.Integrate(request);

            if (!response.IsValid)
            {
                _logger.LogInformation("Falha no processamento do item: {0}", response);
                return false;
            }

            _logger.LogInformation("Processamento realizado com sucesso", request);

            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            return true;
        }
    }
}
