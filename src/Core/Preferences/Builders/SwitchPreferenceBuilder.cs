using Core.Preferences.Controls;

namespace Core.Preferences.Builders;

public sealed class SwitchPreferenceBuilder : PreferenceValueBuilderBase<bool>
{
    public override SwitchPreference Build()
    {
        return new SwitchPreference(this);
    }
}

public static partial class PreferenceBuilderMixins
{
    /// <summary>
    /// Add a <see cref="SwitchPreference"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddSwitch<TBuilder>(this TBuilder builder, string key, Action<SwitchPreferenceBuilder> configure) where TBuilder : PreferenceCollectionBuilderBase
    {
        var @switch = new SwitchPreferenceBuilder { Key = key };
        configure(@switch);
        return builder.AddSwitch(@switch);
    }

    /// <summary>
    /// Add a <see cref="SwitchPreference"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddSwitch<TBuilder>(this TBuilder builder, SwitchPreferenceBuilder @switch) where TBuilder : PreferenceCollectionBuilderBase
    {
        builder.AddPreference(@switch);
        return builder;
    }
}
