using Core.Abstractions;
using FluentValidation;

namespace Core.Primitives;

public readonly record struct Address :
    IEmpty<Address>,
    IValid<Address>,
    IEquatable<Address>,
    IEquatable<Address?>
{
    public required string Street { get; init; }

    public required string Region { get; init; }

    public required string ZipCode { get; init; }

    public required string City { get; init; }

    public required string Country { get; init; }

    public required Location Location { get; init; }

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
        Location = Location.Empty
    };

    public static IValidator<Address> Validator { get; } = InlineValidator.For<Address>(data =>
    {
    });
}
