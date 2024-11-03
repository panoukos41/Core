namespace Core.Preferences.Builders;

public sealed class PreferenceCategoryBuilder : PreferenceCollectionBuilderBase
{
    public override PreferenceCategory Build()
    {
        return new(this);
    }
}

public static partial class PreferenceBuilderMixins
{
    /// <summary>
    /// Add a <see cref="PreferenceCategory"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddCategory<TBuilder>(this TBuilder builder, Action<PreferenceCategoryBuilder> configure) where TBuilder : PreferenceCollectionBuilderBase
    {
        var category = new PreferenceCategoryBuilder();
        configure(category);
        return builder.AddCategory(category);
    }

    /// <summary>
    /// Add a <see cref="PreferenceCategory"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddCategory<TBuilder>(this TBuilder builder, string title, Action<PreferenceCategoryBuilder> configure) where TBuilder : PreferenceCollectionBuilderBase
    {
        var category = new PreferenceCategoryBuilder() { Title = title };
        configure(category);
        return builder.AddCategory(category);
    }

    /// <summary>
    /// Add a <see cref="PreferenceCategory"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    private static TBuilder AddCategory<TBuilder>(this TBuilder builder, PreferenceCategoryBuilder category) where TBuilder : PreferenceCollectionBuilderBase
    {
        builder.AddPreference(category);
        return builder;
    }
}
