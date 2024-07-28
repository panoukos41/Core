using Core.Blazor.Reactive.Forms.Abstract;
using Core.Blazor.Reactive.Forms.Primitives;

namespace Core.Blazor.Reactive.Forms;

public sealed class FormControl<TValue> : AbstractControl<TValue> where TValue : IParsable<TValue>
{
    public FormControl()
    {
    }

    public FormControl(IEnumerable<ValidatorFn>? sync = null, IEnumerable<ValidatorFnAsync>? async = null) : base(sync, async)
    {
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

    /// <summary>
    /// Set raw string value like values provided from input controls.
    /// </summary>
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
        MarkAsPristine();
        MarkAsUnTouched();
        Enable();
        Value = value;
    }
}
