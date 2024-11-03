using Core.Preferences.Builders;
using Core.Reactive;

namespace Core.Preferences.Abstract;

public abstract class PreferenceBase : RxObject
{
    /// <summary>
    /// A custom value to be used by platform integrations to represent the Preference icon.
    /// </summary>
    public string? Icon { get; }

    /// <summary>
    /// A value that represents the title of the Preference.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// A value that represents the Preference summary.
    /// </summary>
    public string? Description { get; }

    protected PreferenceBase(PreferenceBuilderBase options)
    {
        Icon = options.Icon;
        Title = options.Title;
        Description = options.Description;
    }
}
