using Core.Preferences.Controls;
using Microsoft.AspNetCore.Components;

namespace Core.Preferences.Abstract;

public abstract class PreferenceComponentBase<TPreference> : ComponentBase
    where TPreference : PreferenceBase
{
    [CascadingParameter]
    public PreferenceScreen? Root { get; set; }

    [Parameter, EditorRequired]
    public TPreference Preference { get; set; } = null!;

    protected object? GetIcon()
        => Root?.IconTransformer is { } transformer ? transformer(Preference.Icon) : Preference.Icon;

    protected string GetTitle()
        => Root?.TitleTransformer is { } transformer ? transformer(Preference.Title) : Preference.Title;

    protected string? GetDescription()
        => Root?.DescriptionTransformer is { } transformer ? transformer(Preference.Description) : Preference.Description;
}
