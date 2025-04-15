using Core.Abstractions;
using R3;
using System.Runtime.CompilerServices;

namespace Core.Reactive;

public abstract class ObservableObject :
    IWhenPropertyChanging,
    IWhenPropertyChanged,
    IDisposeWith,
    IDisposable
{
    private readonly Lazy<CompositeDisposable> disposables = new(static () => []);

    /// <summary>
    /// A list of disposables to dispose when this instance is disposed.
    /// </summary>
    protected CompositeDisposable Disposables => disposables.Value;

    /// <inheritdoc/>
    public void DisposeWith(IDisposable disposable)
    {
        disposables.Value.Add(disposable);
    }

    #region Raise

    /// <summary>
    /// Notify subscribers that a property is changing.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    public void RaisePropertyChanging([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is not { Length: > 0 }) return;

        changingSubject.Value.OnNext(new PropertyChanging(this, propertyName));
    }

    /// <summary>
    /// Notify subscribers that a property has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is not { Length: > 0 }) return;

        changedSubject.Value.OnNext(new PropertyChanged(this, propertyName));
    }

    /// <summary>
    /// Notify subscribers that properties are changing.
    /// </summary>
    /// <param name="propertyNames">The names of the properties.</param>
    public void RaisePropertiesChanging(params ReadOnlySpan<string> propertyNames)
    {
        if (propertyNames.Length is 0) return;

        foreach (var name in propertyNames)
        {
            RaisePropertyChanging(name);
        }
    }

    /// <summary>
    /// Notify subscribers that a properties have changed.
    /// </summary>
    /// <param name="propertyNames">The names of the properties.</param>
    public void RaisePropertiesChanged(params ReadOnlySpan<string> propertyNames)
    {
        if (propertyNames.Length is 0) return;


        foreach (var name in propertyNames)
        {
            RaisePropertyChanged(name);
        }
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
    protected void SetAndRaise<T>(ref T backingField, T newValue, params ReadOnlySpan<string> properties)
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

    #region IObservalbeObject/IDispose

    public bool Disposed { get; private set; }

    private readonly Lazy<Subject<PropertyChanging>> changingSubject = new(static () => new());
    private readonly Lazy<Subject<PropertyChanged>> changedSubject = new(static () => new());

    /// <inheritdoc/>
    public Observable<PropertyChanging> WhenPropertyChanging => changingSubject.Value;

    /// <inheritdoc/>
    public Observable<PropertyChanged> WhenPropertyChanged => changedSubject.Value;

    /// <inheritdoc/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        Disposed = true;

        if (disposables.IsValueCreated)
        {
            disposables.Value.Dispose();
        }
        if (changingSubject.IsValueCreated)
        {
            changingSubject.Value.Dispose();
        }
        if (changedSubject.IsValueCreated)
        {
            changedSubject.Value.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    #endregion
}
