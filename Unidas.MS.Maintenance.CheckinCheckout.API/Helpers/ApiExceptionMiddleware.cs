using Unidas.MS.Maintenance.CheckinCheckout.Application.Services.Exceptions;
using System.ServiceModel;
using System.Diagnostics.CodeAnalysis;

namespace Unidas.MS.Maintenance.CheckinCheckout.API.Helpers
{
    [ExcludeFromCodeCoverage]
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate next;
        ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(ILogger<ApiExceptionMiddleware> logger, RequestDelegate next)
        {
            this.next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ApiExceptionMiddleware> logger)
        {
            var message = "Não foi possível processar sua requisição, tente mais tarde";

            if (exception is ServiceException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
                logger.LogInformation(exception.Message, exception);
            }
            else if (exception is ActionNotSupportedException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                logger.LogError(exception.Message, exception);
            }
            else if (exception is EndpointNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                logger.LogError(exception.Message, exception);
            }
            else if (exception is FaultException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                logger.LogError(exception.Message, exception);
            }
            else if (exception is TimeoutException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
                logger.LogError(exception.Message, exception);
            }
            else 
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                logger.LogError(exception.Message, exception);
            }

            var result = JsonConvert.SerializeObject(new { error = message });
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(result);
        }
    }
}
