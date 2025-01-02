using Core.Preferences.Components;
using Core.Preferences.Controls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Core.Preferences.Abstract;

public abstract class PreferenceCollectionComponentBase<TPreferenceCollection> : PreferenceComponentBase<TPreferenceCollection>
    where TPreferenceCollection : PreferenceCollectionBase
{
    protected virtual RenderFragment RenderPreference(PreferenceBase preference) => builder =>
    {
        _ = preference switch
        {
            PreferenceCategory categoryPreference => Render<PreferenceCategoryComponent, PreferenceCategory>(ref builder, ref categoryPreference),
            EditTextPreference editTextPreference => Render<EditTextPreferenceComponent, EditTextPreference>(ref builder, ref editTextPreference),
            EditNumberPreference<int> editIntPreference => Render<EditIntPreferenceComponent, EditNumberPreference<int>>(ref builder, ref editIntPreference),
            EditNumberPreference<long> editLongPreference => Render<EditLongPreferenceComponent, EditNumberPreference<long>>(ref builder, ref editLongPreference),
            EditNumberPreference<decimal> editDecimalPreference => Render<EditDecimalPreferenceComponent, EditNumberPreference<decimal>>(ref builder, ref editDecimalPreference),
            EditNumberPreference<double> editDoublePreference => Render<EditDoublePreferenceComponent, EditNumberPreference<double>>(ref builder, ref editDoublePreference),
            SwitchPreference switchPreference => Render<SwitchPreferenceComponent, SwitchPreference>(ref builder, ref switchPreference),
            ListBoxPreference listBoxPreference => Render<ListBoxPreferenceComponent, ListBoxPreference>(ref builder, ref listBoxPreference),
            _ => Nothing.Value
        };
    };

    private static Nothing Render<TComponent, TPreference>(ref RenderTreeBuilder builder, ref TPreference preference)
        where TComponent : notnull, PreferenceComponentBase<TPreference>
        where TPreference : PreferenceBase
    {
        builder.OpenComponent<TComponent>(0);
        builder.AddComponentParameter(1, nameof(Preference), preference);
        builder.CloseComponent();
        return Nothing.Value;
    }
}
