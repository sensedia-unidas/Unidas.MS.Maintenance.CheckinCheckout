using FluentValidation.Results;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services.UseCases;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;
using Microsoft.Extensions.Logging;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Services
{
    public class CheckinCheckoutService : ICheckinCheckoutService
    {
        private readonly ISendToAxUseCase _useCase;
        private readonly ILogger<CheckinCheckoutService> _logger;
        public CheckinCheckoutService(ISendToAxUseCase sendToAxUseCase,
            ILogger<CheckinCheckoutService> logger)
        {
            _useCase = sendToAxUseCase;
            _logger = logger;
        }
        public async Task<ValidationResult> Integrate(ItemCheckinCheckoutRequestViewModel request)
        {
            _logger.LogInformation("Iniciando integração", request);

            var validation = new ValidationResult();

            if (!await _useCase.Execute(request))
            {
                _logger.LogInformation("Integração não realizada", request);
                validation.Errors.Add(new ValidationFailure(String.Empty, "Integração não realizada"));
            }

            _logger.LogInformation("Finalizando integração", request);

            return validation;
        }
    }
}
