using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services.UseCases
{
    public interface ISendToAxUseCase
    {
        Task<bool> Execute(ItemCheckinCheckoutRequestViewModel item);
    }
}
