using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services.UseCases;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.ServiceModel;
using AutoMapper;

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
        //public async Task<bool> Execute(ItemCheckinCheckoutRequestViewModel item)
        //{
        //    try
        //    {
        //        _logger.LogInformation("SendToAxUseCase - Iniciando envio para o AX", item);

        //        //var client = new HttpClient
        //        //{
        //        //    BaseAddress = new Uri(_appSettings.AXConnector.CreateCheckInCheckOutUrl)
        //        //};

        //        //var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
        //        //client.Timeout = new TimeSpan(0, 3, 0);
        //        //var response = await client.PostAsync(_appSettings.AXConnector.CreateCheckInCheckOutUrl, content);

        //        //if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //        //{
        //        //    _logger.LogWarning("SendToAxUseCase - Falha ao tentar enviar para o AX", item, response);

        //        //    return false;
        //        //}

        //        _logger.LogInformation("SendToAxUseCase - Finalizando envio para o AX", item);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendToAxUseCase - Erro ao tentar envio para o AX", ex);
        //        throw;
        //    }
        //}

        public async Task<bool> Execute(ItemCheckinCheckoutRequestViewModel item)
        {
            try
            {
                _logger.LogInformation("SendToAxUseCase - Iniciando envio para o AX", item);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var endpoint = new EndpointAddress(new Uri(_appSettings.AXConnectorSettings.MaintenanceUrl), new UpnEndpointIdentity(_appSettings.AXConnectorSettings.UserPrincipalName));

                var client = new CaseManagementServices.CaseManagementServiceClient(new NetTcpBinding(), endpoint);

                var axViewModelRequest = _mapper.Map<CaseManagementServices.WorkshopCheck>(item);

                client.ClientCredentials.Windows.ClientCredential.Domain = _appSettings.AXConnectorSettings.Domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = _appSettings.AXConnectorSettings.UserDomain;
                client.ClientCredentials.Windows.ClientCredential.Password = _appSettings.AXConnectorSettings.Password;

                var result = await client.createWorkshopCheckAsync(null, axViewModelRequest).ConfigureAwait(false);

                _logger.LogInformation("SendToAxUseCase - Finalizando envio para o AX", item);
                _logger.LogInformation("SendToAxUseCase - resultado ", result);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("SendToAxUseCase - Erro ao tentar envio para o AX", ex);
                throw;
            }
        }
    }
}
