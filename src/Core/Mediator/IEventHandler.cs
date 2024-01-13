namespace Mediator;

public interface IEventHandler
{
    ValueTask Handle(object @event, CancellationToken cancellationToken);
}

public interface IEventHandler<in TEvent> : IEventHandler where TEvent : class
{
    ValueTask Handle(TEvent @event, CancellationToken cancellationToken);

    ValueTask IEventHandler.Handle(object @event, CancellationToken cancellationToken)
        => Handle((TEvent)@event, cancellationToken);
}
