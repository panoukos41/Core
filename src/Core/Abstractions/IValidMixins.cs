using FluentValidation;
using FluentValidation.Results;

namespace Core.Abstractions;

public static class IValidMixins
{
    public static ValidationResult Validate<TValid>(this TValid obj, ValidationContext<TValid>? validationContext = null) where TValid : class, IValid
    {
        validationContext ??= new ValidationContext<TValid>(obj);
        return TValid.Validator.Validate(validationContext);
    }
}
