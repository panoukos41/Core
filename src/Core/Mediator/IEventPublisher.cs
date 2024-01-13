namespace Mediator;

public interface IEventPublisher
{
    public void Publish<TEvent>(TEvent @event) where TEvent : class;
}
