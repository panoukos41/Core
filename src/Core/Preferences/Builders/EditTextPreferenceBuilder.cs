namespace Core.Preferences.Builders;

public sealed class EditTextPreferenceBuilder : PreferenceValueBuilderBase
{
    public override EditTextPreference Build()
    {
        return new(this);
    }
}

public static partial class PreferenceBuilderMixins
{
    /// <summary>
    /// Add an <see cref="EditTextPreference"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddEditText<TBuilder>(this TBuilder builder, string key, Action<EditTextPreferenceBuilder> configure) where TBuilder : PreferenceCollectionBuilderBase
    {
        var editText = new EditTextPreferenceBuilder { Key = key };
        configure(editText);
        return builder.AddEditText(editText);
    }

    /// <summary>
    /// Add an <see cref="EditTextPreference"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddEditText<TBuilder>(this TBuilder builder, EditTextPreferenceBuilder editText) where TBuilder : PreferenceCollectionBuilderBase
    {
        builder.AddPreference(editText);
        return builder;
    }
}
