namespace Core.Abstractions.Events;

public interface IEventHandler<in TEvent>
{
    ValueTask Handle(TEvent @event, CancellationToken cancellationToken);
}
