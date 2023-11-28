using Core.Abstractions;
using FluentValidation;

namespace Core.Commons;

public sealed record Address : IValid
{
    public required string Street { get; init; }

    public required string Region { get; init; }

    public required string ZipCode { get; init; }

    public required string City { get; init; }

    public required string Country { get; init; }

    public required Location Location { get; init; }

    public static Address Empty { get; } = new()
    {
        Street = string.Empty,
        Region = string.Empty,
        City = string.Empty,
        Country = string.Empty,
        ZipCode = string.Empty,
        Location = Location.Empty
    };

    public static IValidator Validator { get; } = InlineValidator.For<Address>(data =>
    {
    });


    public bool Equals(Address? other)
    {
        return other is { } && GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode()
    {
        var hash1 = HashCode.Combine(Street, Region, ZipCode);
        var hash2 = HashCode.Combine(City, Country, Location);
        return HashCode.Combine(hash1, hash2);
    }
}
