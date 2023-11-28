using Core.Abstractions;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace Core.Commons;

public sealed record Location : IValid
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

    public static Location Empty { get; } = new(0, 0);

    public static IValidator Validator { get; } = InlineValidator.For<Location>(data =>
    {
        data.RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        data.RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);
    });

    public bool Equals(Location? other)
    {
        return other is { }
            && Latitude == other.Latitude
            && Longitude == other.Longitude;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Latitude, Longitude);
    }
}
