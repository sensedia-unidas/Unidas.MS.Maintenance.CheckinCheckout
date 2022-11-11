using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services.UseCases
{
    public interface IServiceBusConsumerUseCase
    {
        Task Execute();
    }
}
