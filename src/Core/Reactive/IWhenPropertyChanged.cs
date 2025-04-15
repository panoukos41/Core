using R3;

namespace Core.Reactive;

/// <summary>
/// Notifies clients that a property has changed.
/// </summary>
public interface IWhenPropertyChanged
{
    /// <summary>
    /// Observable that ticks when a property has changed.
    /// </summary>
    public Observable<PropertyChanged> WhenPropertyChanged { get; }
}
