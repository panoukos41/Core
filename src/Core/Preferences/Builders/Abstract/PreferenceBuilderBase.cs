using Core.Preferences.Abstract;

namespace Core.Preferences.Builders;

public abstract class PreferenceBuilderBase
{
    public string? Icon { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public abstract PreferenceBase Build();
}

public static partial class PreferenceBuilderExtensions
{
    public static TBuilder WithIcon<TBuilder>(this TBuilder builder, string? icon)
        where TBuilder : PreferenceBuilderBase
    {
        builder.Icon = icon;
        return builder;
    }

    public static TBuilder WithTitle<TBuilder>(this TBuilder builder, string title)
        where TBuilder : PreferenceBuilderBase
    {
        builder.Title = title;
        return builder;
    }

    public static TBuilder WithDescription<TBuilder>(this TBuilder builder, string? description)
        where TBuilder : PreferenceBuilderBase
    {
        builder.Description = description;
        return builder;
    }
}
