using R3;

namespace Core.Reactive;

/// <summary>
/// Notifies clients that a property is changing.
/// </summary>
public interface IWhenPropertyChanging
{
    /// <summary>
    /// Observable that ticks when a property is changing.
    /// </summary>
    public Observable<PropertyChanging> WhenPropertyChanging { get; }
}
