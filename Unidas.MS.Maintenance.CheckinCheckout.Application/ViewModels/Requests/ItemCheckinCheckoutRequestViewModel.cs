using Newtonsoft.Json.Linq;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests
{
    public class ItemCheckinCheckoutRequestViewModel
    {
        // AMRentDeviceCheckOrigin
        // Eproc = 1
        // App = 2
        // Telemetry = 3
        // SalesForce = 4
        public int CheckOrigin { get; set; }

        // CNPJCPFNum_BR
        public string Document { get; set; }
        public string IntegrationRefId { get; set; }
        public string RegistrationNumber { get; set; }

        // Checkin or Checkout date
        public DateTime WorkshopCheckDate { get; set; }

        // WorkshopCheckType
        // Checkin = 1
        // Checkout = 2
        public int WorkshopCheckType { get; set; }
        public bool WorkshopCustomerConfirmation { get; set; }
        public string WorkshopObservation { get; set; }
        public bool WorkshopServiceCompleted { get; set; }
        public string WorkshopServiceReason { get; set; }
    }
}
