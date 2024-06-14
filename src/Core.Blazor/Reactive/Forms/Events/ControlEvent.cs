using Core.Blazor.Reactive.Forms.Abstract;

namespace Core.Blazor.Reactive.Forms.Events;

/// <summary>
/// Base class for every event sent by `AbstractControl.events()`
/// </summary>
public class ControlEvent : EventArgs
{
    public AbstractControl Sender { get; }

    public ControlEvent(AbstractControl sender)
    {
        Sender = sender;
    }
}
