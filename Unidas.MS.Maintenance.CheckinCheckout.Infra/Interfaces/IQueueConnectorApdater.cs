namespace Unidas.MS.Maintenance.CheckinCheckout.Infra.Interfaces
{
    public interface IQueueConnectorApdater
    {
        Task SendMessage(string sbConnectionString, string sbQueueName, object itemCheckinCheckout);
    }
}
