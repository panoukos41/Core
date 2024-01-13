using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Unit = System.Reactive.Unit;

namespace Core.Common.Hosting;

public sealed class EventHost : BackgroundService, IDisposable
{
    private static readonly Type handlerInterfaceType = typeof(IEventHandler<>);
    private readonly ConcurrentDictionary<Type, Type[]> handlerTypesCache = new();

    private readonly IEventSubscriber eventSubscriber;
    private readonly IServiceProvider serviceProvider;

    public EventHost(IEventSubscriber eventSubscriber, IServiceProvider serviceProvider)
    {
        this.eventSubscriber = eventSubscriber;
        this.serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        eventSubscriber
            .SubscribeOn(TaskPoolScheduler.Default)
            .SelectMany(@event =>
            {
                var eventType = @event.GetType();
                var scope = serviceProvider.CreateScope();
                var services = scope.ServiceProvider;

                var handlerTypes = handlerTypesCache.GetOrAdd(eventType, eventType => FindHandlerTypes(eventType, services).Value);
                if (handlerTypes.Length == 0)
                {
                    scope.Dispose();
                    return Observable.Return(Unit.Default);
                }

                var handlers = handlerTypes
                    .SelectMany(handlerType => services.GetServices(handlerType))
                    .Cast<IEventHandler>();

                return handlers
                    .ToObservable()
                    .SelectMany(handler => Observable
                        .FromAsync(token => handler.Handle(@event, token).AsTask(), TaskPoolScheduler.Default)
                        .Do(_ =>
                        {
                            Log.ForContext(eventType).Information("Event {Event} handled from {Handler}", eventType, handler.GetType());
                        })
                        .Catch<Unit, Exception>(ex =>
                        {
                            if (ex is not TaskCanceledException and not OperationCanceledException)
                            {
                                Log.ForContext(eventType).Error(ex, "Event {Event} error from {Handler}", eventType, handler.GetType());
                            }
                            return Observable.Return(Unit.Default);
                        }))
                    .Finally(scope.Dispose);
            })
            .Subscribe(stoppingToken);

        Log.ForContext<EventHost>().Information("Started Event Host");
        return Task.CompletedTask;
    }

    private static Lazy<Type[]> FindHandlerTypes(Type eventType, IServiceProvider serviceProvider) => new(() =>
    {
        var handlerTypesSearch = GetEventTypes(eventType).ToHashSet();
        var handlerTypesToRemove = new HashSet<Type>();

        var handlers = handlerTypesSearch
            .SelectMany(handlerType =>
            {
                var foundServices = serviceProvider.GetServices(handlerType);
                if (foundServices.Any())
                {
                    return foundServices;
                }
                handlerTypesToRemove.Add(handlerType);
                return Enumerable.Empty<Type>();
            })
            .ToArray();

        return handlerTypesSearch
            .Except(handlerTypesToRemove)
            .ToArray();
    });

    private static IEnumerable<Type> GetEventTypes(Type eventType)
    {
        var interfaces = eventType.GetInterfaces();
        var length = interfaces.Length;
        for (int i = 0; i < length; i++)
        {
            yield return HandlerType(interfaces[i]);
        }

        yield return HandlerType(eventType);

        while (eventType.BaseType is { } baseType)
        {
            yield return HandlerType(baseType);
            eventType = baseType;
        }

        static Type HandlerType(Type eventType) => handlerInterfaceType.MakeGenericType(eventType);
    }
}
