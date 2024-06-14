﻿using Core.Blazor.Reactive.Forms.Events;
using Core.Blazor.Reactive.Forms.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Core.Blazor.Reactive.Forms.Abstract;

/// <summary>
/// Base implementation of all form controls.
/// Contains everything needed for events but needs implementation for validators.
/// </summary>
public abstract class AbstractControl : IDisposable
{
    private readonly Lazy<HashSet<ValidatorFn>> _validators;
    private readonly Lazy<HashSet<ValidatorFnAsync>> _validatorsAsync;
    private IDisposable? _validationSub;

    /// <summary>
    /// Central observable for all events.
    /// </summary>
    protected readonly Subject<ControlEvent> eventsSubject = new();

    /// <summary>
    /// Initialize the AbstractControl instance.
    /// </summary>
    protected AbstractControl()
    {
        Events = eventsSubject.AsObservable();
        StatusChanges = Events.OfType<StatusChangeEvent>();
        ValueChanges = Events.OfType<ValueChangeEvent>();

        _validators = new(static () => []);
        _validatorsAsync = new(static () => []);
    }

    public virtual void Dispose()
    {
        CancelValidation();
        ClearValidators();
        eventsSubject.OnCompleted();
        eventsSubject.Dispose();
        GC.SuppressFinalize(this);
    }

    public abstract object? RawValue { get; }

    /// <summary>
    /// A control is 'enabled' as long as its 'status' is not 'DISABLED'.
    /// </summary>
    public bool Enabled => Status is not ControlStatus.Disabled;

    /// <summary>
    /// A control is 'disabled' when its 'status' is 'DISABLED'.
    /// </summary>
    /// <remarks>
    /// Disabled controls are exempt from validation checks and
    /// are not included in the aggregate value of their ancestor controls.
    /// </remarks>
    public bool Disabled => Status is ControlStatus.Disabled;

    /// <summary>
    /// A control is 'valid' when its 'status' is 'VALID'.
    /// </summary>
    public bool Valid => Status is ControlStatus.Valid;

    /// <summary>
    /// A control is 'invalid' when its 'status' is 'INVALID'.
    /// </summary>
    public bool Invalid => Status is ControlStatus.Invalid;

    /// <summary>
    /// A control is 'pristine' if the user has not yet changed the value in the UI.
    /// </summary>
    /// <value>True if the user has not yet changed the value in the UI.</value>
    /// <remarks>Programmatic changes to a control's value do not mark it dirty.</remarks>
    public bool Pristine { get; private set; } = true;

    /// <summary>
    /// A control is 'dirty' if the user has changed the value in the UI.
    /// </summary>
    /// <value>True if the user has changed the value of this control in the UI.</value>
    /// <remarks>Programmatic changes to a control's value do not mark it dirty.</remarks>
    public bool Dirty => !Pristine;

    /// <summary>
    /// True if the control is marked as 'touched'.
    /// </summary>
    /// <remarks>A control is marked 'touched' once the user has triggered a 'blur' event on it.</remarks>
    public bool Touched { get; private set; }

    /// <summary>
    /// True if the control has not been marked as 'touched'.
    /// </summary>
    /// <remarks>A control is 'untouched' if the user has not yet triggered a 'blur' event on it.</remarks>
    public bool Untouched => !Touched;

    /// <summary>
    /// A control is 'pending' when its 'status' is 'PENDING'.
    /// </summary>
    public bool Pending => Status is ControlStatus.Pending;

    /// <summary>
    /// The status of the control.
    /// </summary>
    public ControlStatus Status { get; private set; }

    /// <summary>
    /// Parent control.
    /// </summary>
    public AbstractControl? Parent { get; internal set; }

    /// <summary>
    /// An object containing any errors generated by failing validation, or null if there are no errors.
    /// </summary>
    public ValidationErrorCollection? Errors { get; protected set; }

    public IEnumerable<ValidatorFn> Validators => _validators.IsValueCreated ? _validators.Value : [];

    public IEnumerable<ValidatorFnAsync> ValidatorsAsync => _validatorsAsync.IsValueCreated ? _validatorsAsync.Value : [];

    /// <summary>
    /// A multicasting observable that emits an event every time the state of the control changes.
    /// It emits for value, status, pristine or touched changes.
    /// </summary>
    /// <remarks>
    /// On value change, the emit happens right after a value of this control is updated. The
    /// value of a parent control(for example if this FormControl is a part of a FormGroup) is updated
    /// later, so accessing a value of a parent control(using the `value` property) from the callback
    /// of this event might result in getting a value that has not been updated yet.
    /// Subscribe to the 'events' of the parent control instead.
    /// For other event types, the events are emitted after the parent control has been updated.
    /// </remarks>
    public IObservable<ControlEvent> Events { get; }

    /// <summary>
    /// A multicasting observable that emits an event every time the validation 'status' of the control recalculates.
    /// </summary>
    public IObservable<StatusChangeEvent> StatusChanges { get; }

    /// <summary>
    /// A multicasting observable that emits an event every time the value of the control changes, in 
    /// the UI or programmatically.It also emits an event each time you call enable() or disable()
    /// without passing along {emitEvent: false} as a function argument.
    /// </summary>
    public virtual IObservable<ValueChangeEvent> ValueChanges { get; }

    public bool Is<T>([NotNullWhen(true)] out AbstractControl<T>? control)
    {
        control = this switch
        {
            AbstractControl<T> c => c,
            _ => null,
        };
        return control is { };
    }

    public ValidationError? GetError(string code, string? path = null)
    {
        throw new NotImplementedException();
    }

    public void SetError(IValidationError error)
    {
        Errors = [error];
    }

    public void SetErrors(IEnumerable<IValidationError> error)
    {
        Errors = [.. error];
    }

    public void ClearErrors()
    {
        Errors = null;
    }

    /**
    * Check whether a synchronous validator function is present on this control. The provided
    * validator must be a reference to the exact same function that was provided.
    *
    * @usageNotes
    *
    * ### Reference to a ValidatorFn
    *
    * ```
    * // Reference to the RequiredValidator
    * const ctrl = new FormControl<number | null>(0, Validators.required);
    * expect(ctrl.hasValidator(Validators.required)).toEqual(true)
    *
    * // Reference to anonymous function inside MinValidator
    * const minValidator = Validators.min(3);
    * const ctrl = new FormControl<number | null>(0, minValidator);
    * expect(ctrl.hasValidator(minValidator)).toEqual(true)
    * expect(ctrl.hasValidator(Validators.min(3))).toEqual(false)
    * ```
    *
    * @param validator The validator to check for presence. Compared by function reference.
    * @returns Whether the provided validator was found on this control.
    */
    public bool HasValidator(ValidatorFn validator)
    {
        if (_validators.IsValueCreated is false) return false;

        return _validators.Value.Contains(new(validator));
    }

    /**
    * Check whether an asynchronous validator function is present on this control. The provided
    * validator must be a reference to the exact same function that was provided.
    *
    * @param validator The asynchronous validator to check for presence. Compared by function
    *     reference.
    * @returns Whether the provided asynchronous validator was found on this control.
    */
    public bool HasValidator(ValidatorFnAsync validator)
    {
        if (_validatorsAsync.IsValueCreated is false) return false;

        return _validatorsAsync.Value.Contains(new(validator));
    }

    /// <summary>
    /// Add a validator to this control, without affecting other validators.
    /// When you add or remove a validator at run time, you must call 'updateValueAndValidity()' for the new validation to take effect.
    /// </summary>
    /// <remarks>
    /// Adding a validator that already exists will have no effect. If duplicate validator functions
    /// are present in the 'validators' array, only the first instance would be added to a form control.
    /// </remarks>
    /// <param name="validator">The new validator function or functions to add to this control.</param>
    public bool AddValidator(ValidatorFn validator)
    {
        return _validators.Value.Add(validator);
    }

    /// <summary>
    /// Add a validator to this control, without affecting other validators.
    /// When you add or remove a validator at run time, you must call 'updateValueAndValidity()' for the new validation to take effect.
    /// </summary>
    /// <remarks>
    /// Adding a validator that already exists will have no effect. If duplicate validator functions
    /// are present in the 'validators' array, only the first instance would be added to a form control.
    /// </remarks>
    /// <param name="validator">The new validator function or functions to add to this control.</param>
    public bool AddValidator(ValidatorFnAsync validator)
    {
        return _validatorsAsync.Value.Add(validator);
    }

    /**
    * Remove a synchronous validator from this control, without affecting other validators.
    * Validators are compared by function reference; you must pass a reference to the exact same
    * validator function as the one that was originally set. If a provided validator is not found,
    * it is ignored.
    *
    * @usageNotes
    *
    * ### Reference to a ValidatorFn
    *
    * ```
    * // Reference to the RequiredValidator
    * const ctrl = new FormControl<string | null>('', Validators.required);
    * ctrl.removeValidators(Validators.required);
    *
    * // Reference to anonymous function inside MinValidator
    * const minValidator = Validators.min(3);
    * const ctrl = new FormControl<string | null>('', minValidator);
    * expect(ctrl.hasValidator(minValidator)).toEqual(true)
    * expect(ctrl.hasValidator(Validators.min(3))).toEqual(false)
    *
    * ctrl.removeValidators(minValidator);
    * ```
    *
    * When you add or remove a validator at run time, you must call
    * `updateValueAndValidity()` for the new validation to take effect.
    *
    * @param validators The validator or validators to remove.
    */
    public bool RemoveValidator(ValidatorFn validator)
    {
        return _validators.IsValueCreated
            && _validators.Value.Remove(validator);
    }


    /**
     * Remove an asynchronous validator from this control, without affecting other validators.
     * Validators are compared by function reference; you must pass a reference to the exact same
     * validator function as the one that was originally set. If a provided validator is not found, it
     * is ignored.
     *
     * When you add or remove a validator at run time, you must call
     * `updateValueAndValidity()` for the new validation to take effect.
     *
     * @param validators The asynchronous validator or validators to remove.
     */
    public bool RemoveValidator(ValidatorFnAsync validator)
    {
        return _validatorsAsync.IsValueCreated
            && _validatorsAsync.Value.Remove(validator);
    }

    /**
     * Empties out the synchronous validator list.
     *
     * When you add or remove a validator at run time, you must call
     * `updateValueAndValidity()` for the new validation to take effect.
     *
     */
    public void ClearValidators()
    {
        ClearValidatorsSync();
        ClearValidatorsAsync();
    }

    /**
     * Empties out the synchronous validator list.
     *
     * When you add or remove a validator at run time, you must call
     * `updateValueAndValidity()` for the new validation to take effect.
     *
     */
    public void ClearValidatorsSync()
    {
        if (_validators is { IsValueCreated: false } or { Value.Count: 0 })
            return;

        _validators.Value.Clear();
    }

    /**
    * Empties out the async validator list.
    *
    * When you add or remove a validator at run time, you must call
    * `updateValueAndValidity()` for the new validation to take effect.
    *
    */
    public void ClearValidatorsAsync()
    {
        if (_validatorsAsync is { IsValueCreated: false } or { Value.Count: 0 })
            return;

        _validators.Value.Clear();
    }

    public void Validate()
    {
        if (Disabled) return;

        // todo: consider using Subject to better throttle and use switching operator to validate.
        if (!_validators.IsValueCreated || _validators.Value.Count is 0)
        {
            SetStatus(ControlStatus.Valid);
            return;
        }

        _validationSub?.Dispose();
        _validationSub = ValidateImpl();
    }

    protected virtual IDisposable? ValidateImpl()
    {
        SetStatus(ControlStatus.Pending);

        var syncValidators = _validators.IsValueCreated is false
            ? Observable.Return<IValidationError?>(null)
            : _validators.Value
            .ToObservable()
            .Select(validator => validator(this));

        var asyncValidators = _validatorsAsync.IsValueCreated is false
            ? Observable.Return<IValidationError?>(null)
            : _validatorsAsync.Value
            .ToObservable()
            .SelectMany((validator) => Observable.FromAsync(cancelation => validator(this, cancelation).AsTask()));

        return Observable
            .Merge(syncValidators, asyncValidators)
            .WhereNotNull()
            .ToArray()
            .Subscribe(errors =>
            {
                if (errors?.Length is > 0)
                {
                    SetErrors(errors);
                    SetStatus(ControlStatus.Invalid);
                    return;
                }
                ClearErrors();
                SetStatus(ControlStatus.Valid);
            });
    }

    public void CancelValidation()
    {
        _validationSub?.Dispose();
        _validationSub = null;
        SetStatus(ControlStatus.Invalid);
    }

    /// <summary>
    /// Retrieves the top-level ancestor of this control.
    /// </summary>
    public AbstractControl? Root()
    {
        var parent = Parent;
        while (parent is not null)
        {
            if (parent.Parent is null) break;
            parent = parent.Parent;
        }
        return parent;
    }

    public void Enable()
    {
        if (Enabled) return;

        SetStatus(ControlStatus.Pending);
        Validate();
    }

    public void Disable()
    {
        SetStatus(ControlStatus.Disabled);
    }

    /// <summary>
    /// Marks the control as 'pristine'. A control is 'pristine' if the user has not yet changed the value in the UI.
    /// </summary>
    /// <value>True if the user has not yet changed the value in the UI.</value>
    /// <remarks>Programmatic changes to a control's value do not mark it dirty.</remarks>
    public void MarkAsPristine()
    {
        SetPristine(true);
    }

    /// <summary>
    /// Marks the control as 'dirty'. A control is 'dirty' if the user has changed the value in the UI.
    /// </summary>
    /// <value>True if the user has changed the value of this control in the UI.</value>
    /// <remarks>Programmatic changes to a control's value do not mark it dirty.</remarks>
    public void MarkAsDirty()
    {
        SetPristine(false);
    }

    /// <summary>
    /// Marks the control as 'touched'. A control is touched
    /// by focus and blur events that do not change the value.
    /// </summary>
    public void MarkAsTouched()
    {
        SetTouched(true);
    }

    /// <summary>
    /// Marks the control as 'untouched'. A control is 'untouched'
    /// if the user has not yet triggered a 'blur' event on it.
    /// </summary>
    public void MarkAsUnTouched()
    {
        SetTouched(false);
    }

    protected Unit SetStatus(ControlStatus status)
    {
        if (Status == status)
            return Unit.Default;

        Status = status;
        if (Status is ControlStatus.Disabled)
        {
            CancelValidation();
        }
        return SendEvent(new StatusChangeEvent(this, Status));
    }

    protected Unit SetPristine(bool pristine)
    {
        if (Pristine == pristine)
            return Unit.Default;

        Pristine = pristine;
        return SendEvent(new PristineChangeEvent(this, Pristine));
    }

    protected Unit SetTouched(bool touched)
    {
        if (Touched == touched)
            return Unit.Default;

        Touched = touched;
        return SendEvent(new TouchedChangeEvent(this, Touched));
    }

    protected Unit SendEvent(ControlEvent @event)
    {
        eventsSubject.OnNext(@event);
        return Unit.Default;
    }
}

/// <summary>
/// Base implementation of all form controls.
/// Contains everything needed for events but needs implementation for validators.
/// Also stores the Value.
/// </summary>
public abstract class AbstractControl<T> : AbstractControl, IDisposable
{
    private T? _value;

    /// <summary>
    /// Initialize the AbstractControl instance.
    /// </summary>
    protected AbstractControl()
    {
        ValueChanges = eventsSubject.OfType<ValueChangeEvent<T>>();
    }

    public override object? RawValue => Value;

    public T? Value
    {
        get => _value;
        set {
            _value = value;
            eventsSubject.OnNext(new ValueChangeEvent<T>(this, _value));
            MarkAsDirty();
            Validate();
        }
    }

    public override IObservable<ValueChangeEvent<T>> ValueChanges { get; }
}
