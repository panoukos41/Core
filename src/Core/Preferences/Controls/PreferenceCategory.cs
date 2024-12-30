using Core.Preferences.Abstract;
using Core.Preferences.Builders;

namespace Core.Preferences.Controls;

public sealed class PreferenceCategory : PreferenceCollectionBase
{
    public PreferenceCategory(PreferenceCategoryBuilder builder) : base(builder)
    {
    }
}
