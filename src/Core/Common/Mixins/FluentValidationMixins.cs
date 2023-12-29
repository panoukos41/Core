using Core.Abstractions;

namespace FluentValidation;

public static class FluentValidationMixins
{
    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> builder)
        => builder
        .NotEmpty()
        .EmailAddress()
        .Length(3, 25)
        .WithName("Email");

    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> builder)
        => builder
        .NotEmpty()
        .MinimumLength(10)
        .WithName("Password");

    public static IRuleBuilderOptions<T, TValid> Valid<T, TValid>(this IRuleBuilder<T, TValid> builder)
        where TValid : IValid<TValid>
        => builder.SetValidator(TValid.Validator);
}
