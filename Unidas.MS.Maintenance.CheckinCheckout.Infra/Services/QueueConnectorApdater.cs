using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Unidas.MS.Maintenance.CheckinCheckout.Infra.Interfaces;

namespace Unidas.MS.Maintenance.CheckinCheckout.Infra.Services
{
    public class QueueConnectorApdater : IQueueConnectorApdater
    {
        public QueueConnectorApdater()
        {
        }

        public async Task SendMessage(string sbConnectionString, string sbQueueName, object itemCheckinCheckout)
        {
            var client = new ServiceBusClient(sbConnectionString);
            var sender = client.CreateSender(sbQueueName);
            var body = JsonSerializer.Serialize(itemCheckinCheckout);
            var message = new ServiceBusMessage(body);
            await sender.SendMessageAsync(message);
        }
    }
}
