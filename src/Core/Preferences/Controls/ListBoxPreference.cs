using Core.Preferences.Abstract;
using Core.Preferences.Builders;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Core.Preferences.Controls;

public sealed class ListBoxPreference : PreferenceValueBase, IReadOnlyList<string>
{
    private readonly string[] allowedValues = [];

    /// <inheritdoc/>
    public int Count => allowedValues.Length;

    /// <inheritdoc/>
    public string this[int index] => allowedValues[index];

    /// <inheritdoc/>
    public override string Value
    {
        get => base.Value;
        set {
            if (allowedValues.Contains(value))
            {
                base.Value = value;
            }
        }
    }

    [SetsRequiredMembers]
    public ListBoxPreference(ListBoxPreferenceBuilder builder) : base(builder)
    {
        allowedValues = [.. builder.AllowedValues];
    }

    /// <inheritdoc/>
    public bool Contains(string item)
    {
        return allowedValues.Contains(item);
    }

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator()
    {
        return allowedValues.AsEnumerable().GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return allowedValues.GetEnumerator();
    }
}
