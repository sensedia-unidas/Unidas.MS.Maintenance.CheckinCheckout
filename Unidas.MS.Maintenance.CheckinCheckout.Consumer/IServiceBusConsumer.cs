namespace Unidas.MS.Maintenance.CheckinCheckout.Consumer
{
    public interface IServiceBusConsumer
    {
        Task ExecuteAsync();
    }
}
