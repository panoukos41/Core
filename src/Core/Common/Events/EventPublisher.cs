using Mediator;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Core.Common.Events;

public sealed class EventPublisher : ObservableBase<object>, IEventPublisher, IEventSubscriber, IDisposable
{
    private readonly Subject<object> _events = new();

    public IObservable<object> Events { get; }

    public EventPublisher()
    {
        Events = _events.AsObservable();
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : class
    {
        _events.OnNext(@event);
    }
    protected override IDisposable SubscribeCore(IObserver<object> observer)
    {
        return Events.Subscribe(observer);
    }

    public void Dispose()
    {
        _events.Dispose();
    }
}
