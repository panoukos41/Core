using Core;
using Core.Commons;

namespace FluentValidation;

public static class FluentValidationMixins
{
    public static IRuleBuilder<T, Uuid> Uuid<T>(this IRuleBuilder<T, Uuid> builder)
        => builder
        .NotEmpty();

    public static IRuleBuilder<T, Guid> Guid<T>(this IRuleBuilder<T, Guid> builder)
        => builder
        .NotEmpty();

    public static IRuleBuilder<T, string> Email<T>(this IRuleBuilder<T, string> builder)
        => builder
        .NotEmpty()
        .EmailAddress()
        .Length(3, 25)
        .WithName("Email");

    public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> builder)
        => builder
        .NotEmpty()
        .MinimumLength(10)
        .WithName("Password");

    public static IRuleBuilder<T, Phone> Phone<T>(this IRuleBuilder<T, Phone> builder)
        => builder
        .NotEmpty()
        .WithName("Phone");
}
