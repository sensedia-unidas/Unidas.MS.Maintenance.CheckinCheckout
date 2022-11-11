namespace Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels
{
    public class AppSettings
    {
        public AXConnectorSettings AXConnectorSettings { get; set; } = new AXConnectorSettings();
        public ServiceBusSettings ServiceBusSettings { get; set; } = new ServiceBusSettings();
    }

    public class AXConnectorSettings
    {
        public string MaintenanceUrl { get; set; }
        public string UserPrincipalName { get; set; }
        public string Domain { get; set; }
        public string UserDomain { get; set; }
        public string Password { get; set; }
    }

    public class ServiceBusSettings
    {
        public string PrimaryConnectionString { get; set; }
        public string SecondConnectionString { get; set; }
        public string QueueName { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondKey { get; set; }
        public string ArmId { get; set; }
    }
 }
