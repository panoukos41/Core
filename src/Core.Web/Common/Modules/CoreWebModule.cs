using Core.Abstractions;
using Core.Common.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Common.Modules;

public sealed class CoreWebModule : IWebModule<CoreWebModule>
{
    /// <summary>
    /// Toggle whether to use the <see cref="EventHost"/> implementation or not. Defaults to <see langword="true"/>
    /// </summary>
    public bool UseEventHost { get; set; } = true;

    public static void Add(WebApplicationBuilder builder, CoreWebModule module)
    {
        if (module.UseEventHost)
        {
            builder.Services.AddHostedService<EventHost>();
        }
    }

    public static void Use(WebApplication app, CoreWebModule module)
    {
    }
}
