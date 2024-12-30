using Core.Preferences.Abstract;
using Core.Preferences.Builders;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Core.Preferences.Controls;

public sealed class EditNumberPreference<TNumber> : PreferenceValueBase<TNumber> where TNumber : notnull, INumber<TNumber>, new()
{
    public TNumber Step { get; }

    public TNumber? Min { get; }

    public TNumber? Max { get; }

    [SetsRequiredMembers]
    public EditNumberPreference(EditNumberPreferenceBuilder<TNumber> builder) : base(builder)
    {
        Step = builder.Step;
        Min = builder.Min;
        Max = builder.Max;
    }
}
