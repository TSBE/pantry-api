using FluentValidation;
using FluentValidation.Results;
using Opw.HttpExceptions;

namespace Pantry.Features.WebFeature
{
    public static class ValidationHelper
    {
        public static void Validate<TVal, TObj>(this TObj obj)
            where TVal : IValidator<TObj>, new()
        {
            var validator = new TVal();
            ValidationResult? result = validator.Validate(obj);
            if (!result.IsValid)
            {
                throw new ValidationErrorException<ValidationFailure>($"Validation error by the object:{obj?.GetType()}!", result.Errors.ToArray());
            }
        }
    }
}
