using Core.Common.Behaviors;
using Core.Common.Events;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Linq;

namespace Core.Common.Modules;

/// <summary>
/// Adds behaviors in the following order:
/// <br/> - <see cref="LogRequestBehavior{TMessage, TResponse}"/> Logs requests.
/// <br/> - <see cref="RunnerBehavior{TMessage, TResponse}"/> Ensures the pipeline runs or transforms it to an Er result.
/// <br/> - <see cref="ValidBehavior{TMessage, TResponse}"/> Runs the <see cref="IValid.Validator"/> of the message.
/// <br/> - <see cref="FluentValidationBehavior{TMessage, TResponse}"/> Runs any registered <see cref="IValidator{T}"/> for the message.
/// <br/>
/// <br/>
/// Adds implementations for event publishing and subscription:
/// <br/> - <see cref="EventPublisher"/> for <see cref="IEventPublisher"/> and <see cref="IEventSubscriber"/>
/// <br/> - <see cref="FluentValidationBehavior{TMessage, TResponse}"/> Runs any registered <see cref="IValidator{T}"/> for the message.
/// </summary>
public sealed class CoreAppModule : IAppModule<CoreAppModule>
{
    public static void Add(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LogRequestBehavior<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(ValidBehavior<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RunnerBehavior<,>));

        services.AddSingleton<EventPublisher>();
        services.AddSingleton<IEventPublisher>(sp => sp.GetRequiredService<EventPublisher>());
        services.AddSingleton<IEventSubscriber>(sp => sp.GetRequiredService<EventPublisher>());
        services.AddScoped<IObservable<object>, IObservableFactoryObject>();
        services.AddScoped(typeof(IObservable<>), typeof(IObservableFactory<>));
    }
}

internal sealed class IObservableFactory<T> : IObservable<T>, IDisposable
{
    private readonly IEventSubscriber eventSubscriber;
    private IDisposable? sub;

    public IObservableFactory(IEventSubscriber eventSubscriber)
    {
        this.eventSubscriber = eventSubscriber;
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        return sub = eventSubscriber.OfType<T>().Subscribe(observer);
    }

    public void Dispose()
    {
        sub?.Dispose();
        sub = null;
    }
}

internal sealed class IObservableFactoryObject : IObservable<object>, IDisposable
{
    private readonly IEventSubscriber eventSubscriber;
    private IDisposable? sub;

    public IObservableFactoryObject(IEventSubscriber eventSubscriber)
    {
        this.eventSubscriber = eventSubscriber;
    }

    public IDisposable Subscribe(IObserver<object> observer)
    {
        return sub = eventSubscriber.Subscribe(observer);
    }

    public void Dispose()
    {
        sub?.Dispose();
        sub = null;
    }
}
