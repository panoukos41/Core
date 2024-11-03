using Core.Preferences.Abstract;
using Core.Preferences.Builders;
using Microsoft.Extensions.Primitives;

namespace Core.Preferences;

public sealed class PagePreference : PreferenceCollectionBase
{
    public string Path { get; }

    public PagePreference(PagePreferenceBuilder builder) : base(builder)
    {
        Path = builder.Path;
    }
}
