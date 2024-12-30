using Core.Preferences.Controls;

namespace Core.Preferences.Builders;

public sealed class PreferenceScreenBuilder : PreferenceCollectionBuilderBase //: List<PreferenceBuilderBase>
{
    private PreferenceScreenBuilder()
    {
    }

    public override PreferenceScreen Build()
    {
        return new PreferenceScreen(this);
    }

    public static PreferenceScreenBuilder CreateEmpty()
    {
        return new();
    }
}
