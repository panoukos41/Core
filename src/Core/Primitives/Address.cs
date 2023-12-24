using Core.Abstractions;
using FluentValidation;

namespace Core.Primitives;

public readonly record struct Address :
    IEmpty<Address>,
    IValid<Address>,
    IEquatable<Address>,
    IEquatable<Address?>
{
    public string? Street { get; init; }

    public string? Region { get; init; }

    public string? ZipCode { get; init; }

    public string? City { get; init; }

    public string? Country { get; init; }

    public Location? Location { get; init; }

    public override string ToString()
    {
        return string.Join(", ", Street, Region, ZipCode, City, Country);
    }

    public bool Equals(Address? other)
    {
        return other is { } && GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, Region, ZipCode, City, Country, Location);
    }

    public static Address Empty { get; } = new()
    {
        Street = string.Empty,
        Region = string.Empty,
        City = string.Empty,
        Country = string.Empty,
        ZipCode = string.Empty,
        Location = Primitives.Location.Empty
    };

    public static IValidator<Address> Validator { get; } = InlineValidator.For<Address>(data =>
    {
        data.RuleFor(x => x.Street)
            .Length(1, 100)
            .When(x => x.Street is { });

        data.RuleFor(x => x.Region)
            .Length(1, 50)
            .When(x => x.Region is { });

        data.RuleFor(x => x.ZipCode)
            .Length(1, 50)
            .When(x => x.ZipCode is { });

        data.RuleFor(x => x.City)
            .Length(1, 50)
            .When(x => x.City is { });

        data.RuleFor(x => x.Country)
            .Length(1, 50)
            .When(x => x.Country is { });

        data.RuleFor(x => x.Location!.Value)
            .SetValidator(Primitives.Location.Validator)
            .OverridePropertyName(nameof(Address.Location))
            .When(x => x.Location is { });
    });
}
