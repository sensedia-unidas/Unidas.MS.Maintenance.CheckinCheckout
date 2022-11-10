using FluentValidation.Results;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces
{
    public interface IMinimalValidator
    {
        ValidationResult Validate<T>(T model);
    }
}
