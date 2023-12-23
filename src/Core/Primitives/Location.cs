using Core.Abstractions;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace Core.Primitives;

public readonly record struct Location :
    IEmpty<Location>,
    IValid<Location>,
    IEquatable<Location>,
    IEquatable<Location?>
{
    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public Location()
    {
    }

    [SetsRequiredMembers]
    public Location(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public bool Equals(Location? other)
    {
        return other is { }
            && Latitude == other.Value.Latitude
            && Longitude == other.Value.Longitude;
    }

    public override string ToString()
    {
        return $"{Latitude}, {Longitude}";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Latitude, Longitude);
    }

    public static Location Empty { get; } = new()
    {
        Latitude = 0,
        Longitude = 0
    };

    public static IValidator<Location> Validator { get; } = InlineValidator.For<Location>(data =>
    {
        data.RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        data.RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);
    });
}
