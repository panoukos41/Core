using Core.Preferences.Abstract;
using Core.Preferences.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Core.Preferences;

public sealed class SwitchPreference : PreferenceValueBase<bool>
{
    [SetsRequiredMembers]
    public SwitchPreference(SwitchPreferenceBuilder builder) : base(builder)
    {
    }
}
