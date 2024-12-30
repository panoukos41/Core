using Core.Preferences.Controls;

namespace Core.Preferences.Builders;

public sealed class PagePreferenceBuilder : PreferenceCollectionBuilderBase
{
    public required string Path { get; set; }

    public override PagePreference Build()
    {
        return new PagePreference(this);
    }
}

public static partial class PreferenceBuilderMixins
{
    /// <summary>
    /// Add a <see cref="PagePreference"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddPage<TBuilder>(this TBuilder builder, string path, Action<PagePreferenceBuilder> configure) where TBuilder : PreferenceCollectionBuilderBase
    {
        var page = new PagePreferenceBuilder { Path = path };
        configure(page);
        return builder.AddPage(page);
    }

    /// <summary>
    /// Add a <see cref="PagePreference"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddPage<TBuilder>(this TBuilder builder, PagePreferenceBuilder page) where TBuilder : PreferenceCollectionBuilderBase
    {
        builder.AddPreference(page);
        return builder;
    }
}
