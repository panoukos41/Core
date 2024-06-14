using Core.Blazor.Reactive.Forms.Abstract;
using Core.Blazor.Reactive.Forms.Events;
using Core.Blazor.Reactive.Forms.Primitives;
using System.Reactive.Linq;

namespace Core.Blazor.Reactive.Forms;

public sealed class FormControl<TValue> : AbstractControl<TValue> where TValue : IParsable<TValue>
{
    public override IObservable<ValueChangeEvent<TValue>> ValueChanges { get; }

    public FormControl() : base()
    {
        ValueChanges = Events.OfType<ValueChangeEvent<TValue>>();
    }

    /// <inheritdoc/>
    public string? GetFormatted()
    {
        return Value switch
        {
            null => null,
            IFormattable f => f.ToString(null, null), // todo: Implement format and IFormatProvider properties.
            _ => Value.ToString(),
        };
    }

    public void SetRawValue(string? rawValue)
    {
        if (rawValue is null)
        {
            Value = default;
            return;
        }
        if (TValue.TryParse(rawValue, null, out var value))
        {
            Value = value;
        }
        // todo: Malformed raw value callback or exception?
    }

    /// <summary>
    /// Resets the control.
    /// </summary>
    public void Reset(TValue? value)
    {
        SetStatus(ControlStatus.Valid);
        Enable();
        MarkAsPristine();
        MarkAsUnTouched();
        Value = value;
    }
}
