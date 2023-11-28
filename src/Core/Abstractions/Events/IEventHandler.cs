namespace Core.Abstractions.Events;

/// <summary>
/// Represents an object that can handle events of <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of the event to handle.</typeparam>
public interface IEventHandler<in TEvent>
{
    /// <summary></summary>
    ValueTask Handle(TEvent @event, CancellationToken cancellationToken);
}
