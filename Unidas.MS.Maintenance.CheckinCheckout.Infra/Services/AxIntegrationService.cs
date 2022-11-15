using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Net;
using System.ServiceModel;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;
using Unidas.MS.Maintenance.CheckinCheckout.Infra.Interfaces;

namespace Unidas.MS.Maintenance.CheckinCheckout.Infra.Services
{
    public class AxIntegrationService : IAxIntegrationService
    {
        public AxIntegrationService()
        {
        }
        public async Task<bool> Execute(CaseManagementServices.WorkshopCheck axViewModelRequest)
        {
            return true;
        }
    }
}
