using FluentValidation.Results;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Validation
{
    public class MinimalValidator : IMinimalValidator
    {
        public ValidationResult Validate<T>(T model)
        {
            var result = new ValidationResult();

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var customAttributes = property.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.ValidationAttribute), true);
                foreach (var attribute in customAttributes)
                {
                    var validationAttribute = attribute as System.ComponentModel.DataAnnotations.ValidationAttribute;
                    if (validationAttribute != null)
                    {
                        var propertyValue = property.CanRead ? property.GetValue(model) : null;
                        var isValid = validationAttribute.IsValid(propertyValue);

                        if (!isValid)
                        {
                            if (result.Errors.Any(x => x.PropertyName.ToUpper() == property.Name))
                            {
                                var error = new ValidationFailure(property.Name, validationAttribute.FormatErrorMessage(property.Name));
                                result.Errors.Add(error);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }    
}
