using Core.Preferences.Controls;
using Microsoft.AspNetCore.Components;

namespace Core.Preferences.Abstract;

public abstract class PreferenceValueComponentBase<TPreference> : PreferenceComponentBase<TPreference>
    where TPreference : PreferenceValueBase
{
    [CascadingParameter]
    public PreferenceScreen? Screen { get; set; }

    protected Uuid Id { get; } = Uuid.NewUuid();

    protected string GetSummary(string value)
        => Root?.SummaryTransformer is { } transformer ? transformer(Preference.SummaryProvider(value)) : Preference.SummaryProvider(value);
}

public abstract class PreferenceValueComponentBase<TPreference, TValue> : PreferenceValueComponentBase<TPreference>
    where TPreference : PreferenceValueBase<TValue>
    where TValue : notnull, IParsable<TValue>, new()
{
}
