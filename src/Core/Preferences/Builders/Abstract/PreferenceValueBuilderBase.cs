using Core.Preferences.Abstract;

namespace Core.Preferences.Builders;

public abstract class PreferenceValueBuilderBase : PreferenceBuilderBase
{
    public required string Key { get; set; }

    public bool Disabled { get; set; }

    public bool Invisible { get; set; }

    public bool Persist { get; set; } = true;

    public string DefaultValue { get; set; } = string.Empty;

    public SummaryProvider? SummaryProvider { get; set; }
}

public abstract class PreferenceValueBuilderBase<TValue> : PreferenceValueBuilderBase
    where TValue : IParsable<TValue>, new()
{
    public new TValue DefaultValue
    {
        get => string.IsNullOrEmpty(base.DefaultValue) ? new() : TValue.Parse(base.DefaultValue, null);
        set => base.DefaultValue = value.ToString() ?? throw new NullReferenceException("Value should not be null");
    }
}
