using Core.Abstractions;

namespace FluentValidation;

public static class FluentValidationMixins
{
    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> builder)
        => builder
        .MinimumLength(3)
            .WithMessage("'{PropertyName}' must be at least {MinLength} characters. You entered {TotalLength} characters.")
        .MaximumLength(50)
            .WithMessage("'{PropertyName}' must be at maximum {MaxLength} characters. You entered '{TotalLength}' characters.")
        .EmailAddress()
        .WithName("Email");

    public static IRuleBuilderOptions<T, string> Username<T>(this IRuleBuilder<T, string> builder)
        => builder
        .MinimumLength(3)
            .WithMessage("'{PropertyName}' must be at least {MinLength} characters. You entered {TotalLength} characters.")
        .MaximumLength(50)
            .WithMessage("'{PropertyName}' must be at maximum {MaxLength} characters. You entered '{TotalLength}' characters.")
        .WithName("Username");

    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> builder)
        => builder
        .MinimumLength(8)
            .WithMessage("'{PropertyName}' must be at least {MinLength} characters. You entered {TotalLength} characters.")
        .MaximumLength(120)
            .WithMessage("'{PropertyName}' must be at maximum {MaxLength} characters. You entered '{TotalLength}' characters.")
        .Must(static password => password.Any(char.IsLower))
            .WithMessage("'{PropertyName}' must contain at least one lower-case letter")
        .Must(static password => password.Any(char.IsUpper))
            .WithMessage("'{PropertyName}' must contain at least one upper-case letter")
        .Must(static password => password.Any(char.IsNumber))
            .WithMessage("'{PropertyName}' must contain at least one number")
        .Must(static password => password.Any(char.IsSymbol) || password.Any(char.IsPunctuation))
            .WithMessage("'{PropertyName}' must contain at least one symbol")
        .WithName("Password");

    public static IRuleBuilderOptions<T, string> Flag<T>(this IRuleBuilder<T, string> builder)
        => builder
        .NotEmpty()
        .Length(3, 50)
        .WithName("Flag");

    public static IRuleBuilderOptions<T, TValid> Valid<T, TValid>(this IRuleBuilder<T, TValid> builder)
        where TValid : IValid<TValid>
        => builder.SetValidator(TValid.Validator);
}
