using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Core.Blazor.Reactive.Forms.Abstract;

namespace Core.Blazor.Reactive.Forms.Primitives;

/// <summary>
/// Default dictionary backed validation error implementation of <see cref="IValidationError"/>.
/// </summary>
public class ValidationError : IValidationError, IReadOnlyDictionary<string, string>, IEquatable<ValidationError>
{
    private readonly Dictionary<string, string>? properties;

    /// <inheritdoc/>
    public string Key { get; }

    /// <inheritdoc/>
    public IEnumerable<string> Keys => properties?.Keys ?? Enumerable.Empty<string>();

    /// <inheritdoc/>
    public IEnumerable<string> Values => properties?.Values ?? Enumerable.Empty<string>();

    /// <inheritdoc/>
    public int Count => properties?.Count ?? 0;

    /// <inheritdoc/>
    public string this[string key] => properties is { }
        ? properties.TryGetValue(key, out var value) ? value : string.Empty
        : string.Empty;

    public ValidationError(string name, Dictionary<string, string>? properties = null)
    {
        Key = name;
        this.properties = properties;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Key;
    }

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return Equals(obj as ValidationError);
    }

    /// <inheritdoc/>
    public bool Equals(ValidationError? other)
    {
        return other is { } && Key == other.Key;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Key);
    }

    public bool ContainsKey(string key)
    {
        return properties?.ContainsKey(key) ?? false;
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
    {
        Unsafe.SkipInit(out value);
        return properties?.TryGetValue(key, out value) ?? false;
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return properties?.GetEnumerator() ?? Enumerable.Empty<KeyValuePair<string, string>>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static bool operator ==(ValidationError left, ValidationError right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValidationError left, ValidationError right)
    {
        return !(left == right);
    }
}
