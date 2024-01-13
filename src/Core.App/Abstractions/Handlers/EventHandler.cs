namespace Core.Abstractions.Handlers;

public abstract class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent : class
{
    public abstract ValueTask Handle(TEvent @event, CancellationToken cancellationToken);
}
