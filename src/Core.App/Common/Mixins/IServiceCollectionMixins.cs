namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionMixins
{
    public static IServiceCollection AddEventHandler<TEvent, THandler>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TEvent : class
        where THandler : IEventHandler<TEvent>
    {
        services.Add(new ServiceDescriptor(typeof(IEventHandler<TEvent>), typeof(THandler), serviceLifetime));
        return services;
    }
}
