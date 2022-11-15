namespace Unidas.MS.Maintenance.CheckinCheckout.Infra.Interfaces
{
    public interface IAxIntegrationService
    {
        Task<bool> Execute(CaseManagementServices.WorkshopCheck axViewModelRequest);
    }
}
