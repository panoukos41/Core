using System.Numerics;

namespace Core.Preferences.Builders;

public sealed class EditNumberPreferenceBuilder<TNumber> : PreferenceValueBuilderBase<TNumber> where TNumber : INumber<TNumber>, new()
{
    public TNumber Step { get; set; } = TNumber.One;

    public TNumber? Min { get; set; }

    public TNumber? Max { get; set; }

    public override EditNumberPreference<TNumber> Build()
    {
        return new(this);
    }
}

public static partial class PreferenceBuilderMixins
{
    /// <summary>
    /// Add an <see cref="EditTextPreference"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddEditNumber<TBuilder, TNumber>(this TBuilder builder, string key, Action<EditNumberPreferenceBuilder<TNumber>> configure)
        where TBuilder : PreferenceCollectionBuilderBase
        where TNumber : notnull, INumber<TNumber>, IParsable<TNumber>, new()
    {
        var editText = new EditNumberPreferenceBuilder<TNumber> { Key = key };
        configure(editText);
        return builder.AddEditNumber(editText);
    }

    /// <summary>
    /// Add an <see cref="EditTextPreference"/> to the <typeparamref name="TBuilder"/>.
    /// </summary>
    public static TBuilder AddEditNumber<TBuilder, TNumber>(this TBuilder builder, EditNumberPreferenceBuilder<TNumber> editText)
        where TBuilder : PreferenceCollectionBuilderBase
        where TNumber : notnull, INumber<TNumber>, IParsable<TNumber>, new()
    {
        builder.AddPreference(editText);
        return builder;
    }
}
