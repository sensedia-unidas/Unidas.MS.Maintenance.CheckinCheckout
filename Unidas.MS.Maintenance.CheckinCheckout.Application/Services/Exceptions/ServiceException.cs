namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Services.Exceptions
{
    public class ServiceException : Exception
    {
        internal ServiceException(string businessMessage)
               : base(businessMessage)
        {
        }
    }
}
