using Unidas.MS.Maintenance.CheckinCheckout.Application.Services.Exceptions;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;
using Unidas.MS.Maintenance.CheckinCheckout.API.Extensions;

namespace Unidas.MS.Maintenance.CheckinCheckout.API.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class ApiExceptionHelper
    {
        public static void UseExceptionHelper(this IApplicationBuilder app, ILogger loggerFactory)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionHandlerFeature != null)
                    {
                        await HandleExceptionAsync(context, loggerFactory, exceptionHandlerFeature.Error);
                    }
                });
            });
        }

        private async static Task HandleExceptionAsync(HttpContext context, ILogger logger, Exception exception)
        {
            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.HttpContext.Request.Path,
                Detail = "Não foi possível processar sua requisição, tente mais tarde",
                Title = "Não foi possível processar sua requisição"
            };

            if (exception is ServiceException)
            {
                logger.LogInformation(exception.Message, exception);
                problemDetails.Title = exception.Message;
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = exception.Message;
            }
            else if (exception is TimeoutException)
            {
                problemDetails.Status = StatusCodes.Status504GatewayTimeout;
                logger.LogError(exception.Message, exception);
            }
            else 
            {
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                logger.LogError(exception.Message, exception);
            }

            context.Response.StatusCode = problemDetails.Status.Value;
            context.Response.ContentType = "application/problem+json";

            var json = JsonConvert.SerializeObject(problemDetails, SerializerSettings.JsonSerializerSettings);
            await context.Response.WriteAsync(json);
        }
    }
}
