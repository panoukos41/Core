using Core.Abstractions;
using FluentValidation.Results;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Core.Reactive;

public abstract class RxObject :
    IWhenProblem,
    IWhenPropertyChanging,
    IWhenPropertyChanged,
    INotifyPropertyChanging,
    INotifyPropertyChanged,
    IDisposable
{
    private readonly Lazy<Subject<Problem>> problemSubject = new(static () => new());
    private readonly Lazy<CompositeDisposable> disposables = new(static () => []);

    /// <inheritdoc/>
    public IObservable<Problem> WhenProblem => problemSubject.Value;

    /// <summary>
    /// Controls whether the <see cref="SetValidateAndRaise{T}(ref T, T, string?)"/> methods
    /// should set the backing field even if validation failed.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool ShouldSetWhenValidationFailed { get; set; } = true;

    /// <summary>
    /// Controls whether to throw a <see cref="ProblemException"/>
    /// if no one observes the <see cref="WhenProblem"/> stream.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool ThrowWhenNoProblemObservers { get; set; } = false;

    /// <summary>
    /// A list of disposables to dispose when this instance is disposed.
    /// </summary>
    protected CompositeDisposable Disposables => disposables.Value;

    /// <summary>
    /// Add a disposable to the <see cref="Disposables"/> list.
    /// </summary>
    protected void DisposeWith(IDisposable disposable)
    {
        disposables.Value.Add(disposable);
    }

    #region SendProblem

    /// <summary>
    /// Sends a problem to the <see cref="WhenProblem"/> stream.
    /// </summary>
    /// <exception cref="Exception">When no observers are listening for the problem.</exception>
    protected void SendProblem(Problem problem)
    {
        if (ThrowWhenNoProblemObservers && !problemSubject.Value.HasObservers)
        {
            throw new Exception("No problem observer have been registered.", new ProblemException(problem));
        }
        problemSubject.Value.OnNext(problem);
    }

    /// <summary>
    /// If the <paramref name="validationResult"/> has errors then a
    /// <see cref="Problems.Validation"/> problem is send to the <see cref="WhenProblem"/> stream.
    /// </summary>
    /// <param name="validationResult">The validation result to check.</param>
    /// <returns><see langword="true"/> if <paramref name="validationResult"/> had errors otherwise <see langword="false"/>.</returns>
    protected bool SendProblem(ValidationResult validationResult)
    {
        if (validationResult.Errors is { Count: > 0 } errors)
        {
            SendProblem(Problems.Validation.WithValidationFailures(errors));
            return true;
        }
        return false;
    }

    #endregion

    #region Raise

    /// <summary>
    /// Notify subscribers that a property is changing.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    protected void RaisePropertyChanging([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is not { Length: > 0 }) return;

        ChangingHandler?.Invoke(this, new(propertyName));
        changingSubject.OnNext(new PropertyChanging(this, propertyName));
    }

    /// <summary>
    /// Notify subscribers that a property has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is not { Length: > 0 }) return;

        ChangedHandler?.Invoke(this, new(propertyName));
        changedSubject.OnNext(new PropertyChanged(this, propertyName));
    }

    #endregion

    #region SetAndRaise

    /// <summary>
    /// Replace the <paramref name="backingField"/> with the <paramref name="newValue"/>
    /// if they are not equal and raise changing/changed events.
    /// </summary>
    /// <typeparam name="T">The type of the fields.</typeparam>
    /// <param name="backingField">The field that stores the value.</param>
    /// <param name="newValue">The value to replace the backing field with.</param>
    /// <param name="propertyName">Optionally the name of the property that changed.</param>
    protected void SetAndRaise<T>(ref T backingField, T newValue, [CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        if (EqualityComparer<T>.Default.Equals(backingField, newValue))
        {
            return;
        }

        RaisePropertyChanging(propertyName);
        backingField = newValue;
        RaisePropertyChanged(propertyName);
    }

    /// <summary>
    /// Replace the <paramref name="backingField"/> with the <paramref name="newValue"/>
    /// if they are not equal and raise changing/changed events for multiple properties.
    /// </summary>
    /// <typeparam name="T">The type of the fields.</typeparam>
    /// <param name="backingField">The field that stores the value.</param>
    /// <param name="newValue">The value to replace the backing field with.</param>
    /// <param name="properties">The names of the properties that changed.</param>
    protected void SetAndRaise<T>(ref T backingField, T newValue, params string[] properties)
    {
        if (properties is not { Length: > 0 })
        {
            throw new ArgumentException("Properties cant be empty.", nameof(properties));
        }

        if (EqualityComparer<T>.Default.Equals(backingField, newValue))
        {
            return;
        }

        foreach (var propertyName in properties)
        {
            RaisePropertyChanging(propertyName);
        }
        backingField = newValue;
        foreach (var propertyName in properties)
        {
            RaisePropertyChanged(propertyName);
        }
    }

    #endregion

    #region SetValidateAndRaise

    /// <summary>
    /// Replace the <paramref name="backingField"/> with the <paramref name="newValue"/>
    /// if they are not equal then validate the new field and send problems to the stream if any
    /// and lastly raise changing/changed events.
    /// </summary>
    /// <typeparam name="T">The type of the fields that also implements IValid and will be validated.</typeparam>
    /// <param name="backingField">The field that stores the value.</param>
    /// <param name="newValue">The value to replace the backing field with.</param>
    /// <param name="propertyName">Optionally the name of the property that changed.</param>
    protected void SetValidateAndRaise<T>(ref T backingField, T newValue, [CallerMemberName] string? propertyName = null)
       where T : class, IValid<T>
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        if (EqualityComparer<T>.Default.Equals(backingField, newValue)) return;
        if (SendProblem(newValue.Validate()) && !ShouldSetWhenValidationFailed) return;

        RaisePropertyChanging(propertyName);
        backingField = newValue;
        RaisePropertyChanged(propertyName);
    }

    /// <summary>
    /// Replace the <paramref name="backingField"/> with the <paramref name="newValue"/>
    /// if they are not equal then validate the new field and send problems to the stream if any
    /// and lastly raise changing/changed events.
    /// </summary>
    /// <typeparam name="T">The type of the fields that also implements IValid and will be validated.</typeparam>
    /// <param name="backingField">The field that stores the value.</param>
    /// <param name="newValue">The value to replace the backing field with.</param>
    /// <param name="properties">The names of the properties that changed.</param>
    protected void SetValidateAndRaise<T>(ref T backingField, T newValue, params string[] properties)
        where T : class, IValid<T>
    {
        if (properties is not { Length: > 0 })
        {
            throw new ArgumentException("Properties cant be empty.", nameof(properties));
        }

        if (EqualityComparer<T>.Default.Equals(backingField, newValue)) return;
        if (SendProblem(newValue.Validate()) && !ShouldSetWhenValidationFailed) return;

        foreach (var propertyName in properties)
        {
            RaisePropertyChanging(propertyName);
        }
        backingField = newValue;
        foreach (var propertyName in properties)
        {
            RaisePropertyChanged(propertyName);
        }
    }

    #endregion

    #region INotify/IReactiveObject/IDispose

    private bool disposed;

    private readonly Subject<PropertyChanging> changingSubject = new();
    private readonly Subject<PropertyChanged> changedSubject = new();
    private event PropertyChangingEventHandler? ChangingHandler;
    private event PropertyChangedEventHandler? ChangedHandler;

    event PropertyChangingEventHandler? INotifyPropertyChanging.PropertyChanging
    {
        add => ChangingHandler += value;
        remove => ChangingHandler -= value;
    }

    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add => ChangedHandler += value;
        remove => ChangedHandler -= value;
    }

    /// <inheritdoc/>
    public IObservable<PropertyChanging> WhenPropertyChanging => changingSubject;

    /// <inheritdoc/>
    public IObservable<PropertyChanged> WhenPropertyChanged => changedSubject;

    public virtual void Dispose()
    {
        if (disposed) return;
        disposed = true;

        if (disposables.IsValueCreated)
        {
            disposables.Value.Dispose();
        }
        ChangingHandler = null;
        ChangedHandler = null;
        changingSubject.OnCompleted();
        changingSubject.Dispose();
        changedSubject.OnCompleted();
        changedSubject.Dispose();

        GC.SuppressFinalize(this);
    }

    #endregion
}
