using Core.Blazor.Reactive.Forms.Abstract;

namespace Core.Blazor.Reactive.Forms.Events;

/// <summary>
/// Event fired when the control's touched status changes (touched <=> untouched).
/// </summary>
public sealed class TouchedChangeEvent : ControlEvent
{
    public bool Touched { get; }

    public TouchedChangeEvent(AbstractControl sender, bool touched) : base(sender)
    {
        Touched = touched;
    }
}
