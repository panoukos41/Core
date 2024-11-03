namespace Core.Preferences.Builders;

public abstract class PreferenceCollectionBuilderBase : PreferenceBuilderBase
{
    public List<PreferenceBuilderBase> Preferences { get; set; } = [];
}

public static partial class PreferenceBuilderMixins
{
    /// <summary>
    /// Add a preference to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddPreference<TBuilder>(this TBuilder builder, PreferenceBuilderBase preference) where TBuilder : PreferenceCollectionBuilderBase
    {
        builder.Preferences.Add(preference);
        return builder;
    }
}
