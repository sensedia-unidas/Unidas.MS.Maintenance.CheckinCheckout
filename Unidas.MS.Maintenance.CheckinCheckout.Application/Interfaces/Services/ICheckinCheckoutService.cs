using FluentValidation.Results;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services
{
    public interface ICheckinCheckoutService
    {
        Task<ValidationResult> Integrate(ItemCheckinCheckoutRequestViewModel request);
    }
}
