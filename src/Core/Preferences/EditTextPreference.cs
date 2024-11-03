using Core.Preferences.Abstract;
using Core.Preferences.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Core.Preferences;

public sealed class EditTextPreference : PreferenceValueBase
{
    [SetsRequiredMembers]
    public EditTextPreference(EditTextPreferenceBuilder builder) : base(builder)
    {
    }
}
