namespace Core.Reactive;

/// <summary>
/// Notifies clients that a property has changed.
/// </summary>
public interface IWhenPropertyChanged
{
    /// <summary>
    /// Observable that ticks when a property has changed.
    /// </summary>
    public IObservable<PropertyChanged> WhenPropertyChanged { get; }
}
