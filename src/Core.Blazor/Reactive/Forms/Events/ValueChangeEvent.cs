using Core.Blazor.Reactive.Forms.Abstract;

namespace Core.Blazor.Reactive.Forms.Events;

/// <summary>
/// Event fired when the value of a control changes.
/// </summary>
public class ValueChangeEvent : ControlEvent
{
    public ValueChangeEvent(AbstractControl sender) : base(sender)
    {
    }
}

/// <summary>
/// Event fired when the value of a control changes.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed class ValueChangeEvent<T> : ValueChangeEvent
{
    public T? Value { get; }

    public ValueChangeEvent(AbstractControl sender, T? value) : base(sender)
    {
        Value = value;
    }
}
