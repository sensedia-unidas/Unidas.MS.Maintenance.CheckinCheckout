using Unidas.MS.Maintenance.CheckinCheckout.Application.AutoMapper;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services.UseCases;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Services.UseCases;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Validation;
using Microsoft.Extensions.DependencyInjection;
using Unidas.MS.Maintenance.CheckinCheckout.Infra.Interfaces;
using Unidas.MS.Maintenance.CheckinCheckout.Infra.Services;

namespace Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC
{
    public class NativeInjector
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //REPOSITORY

            //ADAPTER
            services.AddScoped<IQueueConnectorApdater, QueueConnectorApdater>();            

            //SERVICE
            services.AddScoped<ICheckinCheckoutService, CheckinCheckoutService>();
            services.AddScoped<ISendToAxUseCase, SendToAxUseCase>();

            //VALIDATOR
            services.AddScoped<IMinimalValidator, MinimalValidator>();

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainToViewModelMappingProfile());
                cfg.AddProfile(new ViewModelToDomainMappingProfile());
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}