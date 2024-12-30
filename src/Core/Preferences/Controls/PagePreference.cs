using Core.Preferences.Abstract;
using Core.Preferences.Builders;

namespace Core.Preferences.Controls;

public sealed class PagePreference : PreferenceCollectionBase
{
    public string Path { get; }

    public PagePreference(PagePreferenceBuilder builder) : base(builder)
    {
        Path = builder.Path;
    }
}
