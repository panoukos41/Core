using Core.Abstractions.Events;

namespace Core.Abstractions.Handlers;

public abstract class EventHandler<TEvent> : IEventHandler<TEvent>
{
    public abstract ValueTask Handle(TEvent @event, CancellationToken cancellationToken);
}
