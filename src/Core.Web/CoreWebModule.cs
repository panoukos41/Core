using Microsoft.AspNetCore.Builder;

namespace Core;

/// <summary>
/// Adds the <see cref="CoreAppModule"/>
/// </summary>
public class CoreWebModule : IWebModule<CoreWebModule>
{
    public static void Add(WebApplicationBuilder builder, CoreWebModule module)
    {
        builder.Services.AddAppModule<CoreAppModule>(builder.Configuration);
    }

    public static void Use(WebApplication app)
    {
    }
}
