using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services.UseCases;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.ServiceModel;
using AutoMapper;
using System.ServiceModel.Description;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Services.UseCases
{
    public class SendToAxUseCase : ISendToAxUseCase
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<SendToAxUseCase> _logger;
        private readonly IMapper _mapper;

        public SendToAxUseCase(AppSettings appSettings,
            ILogger<SendToAxUseCase> logger,
            IMapper mapper)
        {
            _appSettings = appSettings;
            _logger = logger;
            _mapper = mapper;
        }        

        public async Task<bool> Execute(ItemCheckinCheckoutRequestViewModel item)
        {
            _logger.LogInformation("Iniciando envio para o AX", item);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var axViewModelRequest = CreateWorkshopCheckObject(item);

            var client = CreateClient();
            
            try
            {                
                var result = await client.createWorkshopCheckAsync(null, axViewModelRequest).ConfigureAwait(false);
                _logger.LogInformation("Finalizando envio para o AX, resultado {0}", result.ToString());
                return true;
            }
            catch (Exception ex)
            {
                client.Abort();
                _logger.LogError("Erro ao tentar envio para o AX - {0}", ex.Message.ToString());
                return false;
            }
            
        }               

        private CaseManagementServices.CaseManagementServiceClient CreateClient()
        {
            var endpoint = new EndpointAddress(new Uri(_appSettings.AXConnectorSettings.MaintenanceUrl), new UpnEndpointIdentity(_appSettings.AXConnectorSettings.UserPrincipalName));

            var client = new CaseManagementServices.CaseManagementServiceClient(new NetTcpBinding(), endpoint);            

            client.ClientCredentials.Windows.ClientCredential.Domain = _appSettings.AXConnectorSettings.Domain;
            client.ClientCredentials.Windows.ClientCredential.UserName = _appSettings.AXConnectorSettings.UserDomain;
            client.ClientCredentials.Windows.ClientCredential.Password = _appSettings.AXConnectorSettings.Password;

            return client;
        }

        private string FormatDocument(string document)
        {
            if (!string.IsNullOrEmpty(document))
            {
                document = document.Replace(".", "");
                document.Insert(document.Length - 2, "-");
            }

            return document;
        }

        public CaseManagementServices.WorkshopCheck CreateWorkshopCheckObject(ItemCheckinCheckoutRequestViewModel item)
        {
            var axViewModelRequest = new CaseManagementServices.WorkshopCheck()
            {
                CheckOrigin = (CaseManagementServices.AMRentDeviceCheckOrigin)item.CheckOrigin,
                DriverId = FormatDocument(item.Document),
                IntegrationRefId = item.IntegrationRefId,
                RegistrationNumber = item.RegistrationNumber,
                WorkshopCheckDate = item.WorkshopCheckDate,
                WorkshopCheckType = (CaseManagementServices.WorkshopCheckType)item.WorkshopCheckType,
                WorkshopCustomerConfirmation = item.WorkshopCustomerConfirmation ? CaseManagementServices.NoYes.Yes : CaseManagementServices.NoYes.No,
                WorkshopObservation = item.WorkshopObservation,
                WorkshopServiceCompleted = item.WorkshopServiceCompleted ? CaseManagementServices.NoYes.Yes : CaseManagementServices.NoYes.No,
                WorkshopServiceReason = item.WorkshopServiceReason
            };

            return axViewModelRequest;
        }

        //public static EndpointAddress CreateEndpoint(string uriString, EndpointIdentity endpointIdentity)
        //{
        //    return new EndpointAddress(new Uri(uriString), endpointIdentity);
        //}

        //public static EndpointIdentity CreateUpnIdentity(string upnName)
        //{
        //    return new UpnEndpointIdentity(upnName);
        //}
    }
}
