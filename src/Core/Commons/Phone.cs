using Core.Commons.JsonConverters;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Core.Commons;

[JsonConverter(typeof(PhoneJsonConverter))]
public struct Phone : ISpanParsable<Phone>, IEquatable<Phone>, IEquatable<Phone?>
{
    private static readonly SearchValues<char> separatorSearch = SearchValues.Create([' ']);
    private string? _formatted;

    public static Phone Empty { get; } = new(string.Empty, string.Empty);

    public required string CallingCode { get; init; }

    public required string Number { get; init; }

    public string Formatted => _formatted ??= $"{CallingCode} {Number}";

    [SetsRequiredMembers]
    public Phone(string callingCode, string number)
    {
        CallingCode = callingCode;
        Number = number;
    }

    public override string ToString()
    {
        return Formatted;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Phone result)
    {
        Unsafe.SkipInit(out result);

        var separatorIndex = s.IndexOfAny(separatorSearch);
        if (separatorIndex == -1) return false;

        var hasPlus = s.StartsWith("+");
        result = new Phone
        {
            CallingCode = $"{(hasPlus ? null : '+')}{s[..(separatorIndex)]}",
            Number = s[(separatorIndex + 1)..].ToString()
        };
        return true;
    }

    #region Equals/Hashcode

    /// <inheritdoc/>
    public override readonly bool Equals(object? other)
    {
        return other is Phone phone && Equals(phone);
    }

    /// <inheritdoc/>
    public readonly bool Equals(Phone? other)
    {
        return other is { } phone
            && phone.CallingCode == CallingCode
            && phone.Number == Number;
    }

    /// <inheritdoc/>
    public readonly bool Equals(Phone other)
    {
        return other is { } phone
            && phone.CallingCode == CallingCode
            && phone.Number == Number;
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(CallingCode, Number);
    }

    #endregion

    #region ISpanParsable Forwards

    public static Phone Parse(string s, IFormatProvider? provider = null) => Parse(s.AsSpan(), provider);

    public static Phone Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
    {
        return TryParse(s, provider, out var result)
            ? result
            : throw new InvalidOperationException("The provided phone number could not be parsed.");
    }

    public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out Phone result) => TryParse(s, null, out result);

    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out Phone result) => TryParse(s.AsSpan(), null, out result);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Phone result) => TryParse(s.AsSpan(), null, out result);

    #endregion

    #region Operators

    public static bool operator ==(Phone left, Phone right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Phone left, Phone right)
    {
        return !(left == right);
    }

    #endregion
}
