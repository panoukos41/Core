namespace Core.Preferences.Builders;

public sealed class ListBoxPreferenceBuilder : PreferenceValueBuilderBase
{
    private HashSet<string> allowedValues = [];

    public IEnumerable<string> AllowedValues
    {
        get => allowedValues;
        set => allowedValues = [.. value];
    }

    public ListBoxPreferenceBuilder AddOption(string option)
    {
        allowedValues.Add(option);
        return this;
    }

    public ListBoxPreferenceBuilder AddOptions(IEnumerable<string> options)
    {
        allowedValues.AddRange(options);
        return this;
    }

    public override ListBoxPreference Build()
    {
        return new(this);
    }
}

public static partial class PreferenceBuilderMixins
{
    /// <summary>
    /// Add a <see cref="ListBoxPreference"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddListBox<TBuilder>(this TBuilder builder, string key, Action<ListBoxPreferenceBuilder> configure) where TBuilder : PreferenceCollectionBuilderBase
    {
        var listbox = new ListBoxPreferenceBuilder { Key = key };
        configure(listbox);
        return builder.AddListBox(listbox);
    }

    /// <summary>
    /// Add a <see cref="ListBoxPreference"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddListBox<TBuilder>(this TBuilder builder, ListBoxPreferenceBuilder listbox) where TBuilder : PreferenceCollectionBuilderBase
    {
        builder.AddPreference(listbox);
        return builder;
    }
}
