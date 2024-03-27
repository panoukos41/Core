using Core.Abstractions;
using Core.Common.Events;
using Core.Common.Hosting;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Common.Modules;

public sealed class CoreWebModule : IWebModule<CoreWebModule>
{
    /// <summary>
    /// Toggle whether to use the <see cref="EventHost"/> implementation or not. Defaults to <see langword="true"/>
    /// </summary>
    public bool UseEventHost { get; set; } = true;

    public static void Add(WebApplicationBuilder builder, CoreWebModule module)
    {
        builder.Services.AddSingleton<EventPublisher>();
        builder.Services.AddSingleton<IEventPublisher>(static sp => sp.GetRequiredService<EventPublisher>());
        builder.Services.AddSingleton<IEventSubscriber>(static sp => sp.GetRequiredService<EventPublisher>());

        if (module.UseEventHost)
        {
            builder.Services.AddHostedService<EventHost>();
        }

        builder.Services.AddHttpContextAccessor();
        builder.Services.TryAddScoped(sp => sp.GetRequiredService<IHttpContextAccessor>().HttpContext!);
        builder.Services.TryAddScoped(sp => sp.GetRequiredService<HttpContext>().User);
    }

    public static void Use(WebApplication app, CoreWebModule module)
    {
    }
}
