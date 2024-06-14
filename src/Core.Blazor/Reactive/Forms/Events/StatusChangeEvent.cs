using Core.Blazor.Reactive.Forms.Abstract;
using Core.Blazor.Reactive.Forms.Primitives;

namespace Core.Blazor.Reactive.Forms.Events;

/// <summary>
/// Event fired when the control's status changes.
/// </summary>
public sealed class StatusChangeEvent : ControlEvent
{
    public ControlStatus Status { get; }

    public StatusChangeEvent(AbstractControl sender, ControlStatus status) : base(sender)
    {
        Status = status;
    }
}
