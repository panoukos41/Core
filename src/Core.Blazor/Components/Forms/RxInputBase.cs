using Core.Abstractions;
using Core.Blazor.Reactive.Forms;
using Core.Blazor.Reactive.Forms.Events;
using Core.Blazor.Reactive.Forms.Primitives;
using Microsoft.AspNetCore.Components;
using R3;

namespace Core.Components.Forms;

public abstract class RxInputBase<T> : CoreComponent, IDisposable where T : IParsable<T>
{
    private FormControl<T>? __formControl;
    private IDisposable? subscription;

    public FormControl<T> FormControl
    {
        get {
            if (__formControl is { }) return __formControl;

            if (FormGroup?.TryGet(Id, out __formControl) is false)
            {
                FormGroup.Add(Id, __formControl ??= new());
                return __formControl;
            }
            if (FormGroupParent?.TryGet(Id, out __formControl) is false)
            {
                FormGroupParent.Add(Id, __formControl ??= new());
                return __formControl;
            }
            return __formControl ??= new();
        }
    }

    public string? CssClass => FormControl.Status switch
    {
        ControlStatus.Valid => this.Class("valid"),
        ControlStatus.Pending => this.Class("pending"),
        ControlStatus.Invalid => this.Class("invalid"),
        ControlStatus.Disabled => this.Class("disabled"),
        _ => null
    };

    [CascadingParameter]
    public FormGroup? FormGroupParent { get; set; }

    [Parameter]
    public FormGroup? FormGroup { get; set; }

    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public T? Value { get; set; }

    [Parameter]
    public EventCallback<T?> ValueChanged { get; set; }

    [Parameter]
    public bool Read { get; set; }

    [Parameter]
    public HashSet<ValidatorFn>? Validators { get; set; }

    [Parameter]
    public HashSet<ValidatorFnAsync>? ValidatorsAsync { get; set; }

    [Parameter]
    public bool Disabled
    {
        get => FormControl.Disabled;
        set {
            if (value)
            {
                FormControl.ClearErrors();
                FormControl.Disable();
            }
            else
                FormControl.Enable();
        }
    }

    [Parameter]
    public bool HideErrors { get; set; }

    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            Id = $"rx-{Guid.NewGuid():N}";
        }

        subscription = FormControl.Events.Subscribe(e =>
        {
            if (e is ValueChangeEvent<T> valueChange)
            {
                Value = valueChange.Value;
                ValueChanged.InvokeAsync(Value);
                Update();
            }
        });
    }

    protected override void OnParametersSet()
    {
        // If values are not the same this was an update to the [Parameter] so update the control.
        if (!EqualityComparer<T>.Default.Equals(Value, FormControl.Value))
        {
            FormControl.Value = Value;
        }
        // Update validators based on component.
        if (Validators is { } && Validators.SetEquals(FormControl.Validators) is false)
        {
            var toRemove = FormControl.Validators.Except(Validators);
            foreach (var validator in toRemove)
            {
                FormControl.RemoveValidator(validator);
            }
            foreach (var validator in Validators)
            {
                FormControl.AddValidator(validator);
            }
            FormControl.Validate();
        }
        // Update validators based on component.
        if (ValidatorsAsync is { } && ValidatorsAsync.SetEquals(FormControl.ValidatorsAsync) is false)
        {
            var toRemove = FormControl.ValidatorsAsync.Except(ValidatorsAsync);
            foreach (var validator in toRemove)
            {
                FormControl.RemoveValidator(validator);
            }
            foreach (var validator in ValidatorsAsync)
            {
                FormControl.AddValidator(validator);
            }
            FormControl.Validate();
        }
    }

    protected override void OnDispose()
    {
        FormGroupParent?.Remove(Id);
        subscription?.Dispose();
        subscription = null;
    }
}
