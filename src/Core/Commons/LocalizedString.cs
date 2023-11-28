namespace Core.Commons;

/// <summary>
/// A dictionary implementation to store multiple localized string.
/// Use <see cref="string.Empty"/> to set the default value.
/// </summary>
public sealed class LocalizedString : Dictionary<string, string>, IEquatable<Dictionary<string, string>>, IEquatable<LocalizedString>
{
    /// <summary>
    /// Gets or sets the default value.
    /// If the value is null then the key is removed.
    /// If the value does not exist null is returned.
    /// </summary>
    public string? Default
    {
        get => this[string.Empty];
        set => this[string.Empty] = value;
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// If the value is null then the key is removed.
    /// If the value does not exist null is returned.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>The value for the key or null if it doesn't exist.</returns>
    public new string? this[string key]
    {
        get => this.GetValueOrDefault(key);
        set
        {
            if (value is null) Remove(key);
            else base[key] = value;
        }
    }

    /// <inheritdoc/>
    /// <remarks>The <see cref="StringComparer.OrdinalIgnoreCase"/> is used for they keys.</remarks>
    public LocalizedString() : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    /// <inheritdoc/>
    /// <remarks>The <see cref="StringComparer.OrdinalIgnoreCase"/> is used for they keys.</remarks>
    public LocalizedString(IDictionary<string, string> dictionary) : base(dictionary, StringComparer.OrdinalIgnoreCase)
    {
    }

    /// <inheritdoc/>
    /// <remarks>The <see cref="StringComparer.OrdinalIgnoreCase"/> is used for they keys.</remarks>
    public LocalizedString(int capacity) : base(capacity, StringComparer.OrdinalIgnoreCase)
    {
    }

    /// <summary>
    /// Get the <see cref="Default"/> string.
    /// </summary>
    /// <returns>The <see cref="Default"/> string.</returns>
    public override string? ToString()
    {
        return Default;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Dictionary<string, string> otherDict && Equals(otherDict);
    }

    /// <inheritdoc/>
    public bool Equals(LocalizedString? other)
    {
        return Equals(other as Dictionary<string, string>);
    }

    /// <inheritdoc/>
    public bool Equals(Dictionary<string, string>? other)
    {
        // todo: Improve to keys & values. Consider adding methods to get the differences.
        return other?.Keys.ToHashSet().SetEquals(Keys) ?? false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this.Aggregate(0, (acc, current) => HashCode.Combine(acc, current.Key, current.Value));
    }
}
