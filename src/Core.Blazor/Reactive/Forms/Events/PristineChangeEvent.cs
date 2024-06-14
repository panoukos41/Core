using Core.Blazor.Reactive.Forms.Abstract;

namespace Core.Blazor.Reactive.Forms.Events;

/// <summary>
/// Event fired when the control's pristine state changes (pristine <=> dirty).
/// </summary>
public sealed class PristineChangeEvent : ControlEvent
{
    public bool Pristine { get; }

    public PristineChangeEvent(AbstractControl sender, bool pristine) : base(sender)
    {
        Pristine = pristine;
    }
}
