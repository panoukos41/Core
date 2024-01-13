using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Core.Abstractions;

public static class IWebModuleMixins
{
    private static readonly Dictionary<Type, object> modules = [];

    public static IReadOnlyDictionary<Type, object> Modules => new ReadOnlyDictionary<Type, object>(modules);

    public static void AddAppModule<TAppModule>(this WebApplicationBuilder builder, Action<TAppModule>? configure = null)
        where TAppModule : class, IAppModule<TAppModule>, new()
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddAppModule(configuration, configure);
    }

    public static void AddWebModule<TWebModule>(this WebApplicationBuilder builder, Action<TWebModule>? configure = null)
        where TWebModule : class, IWebModule<TWebModule>, new()
    {
        var moduleType = typeof(TWebModule);
        if (modules.ContainsKey(moduleType))
            return;

        var module = new TWebModule();
        modules.Add(moduleType, module);
        builder.Services.AddSingleton(module);
        TWebModule.Add(builder, module);
        Log.ForContext<TWebModule>().Debug("Added WebModule: {Module}", moduleType.Name);
        configure?.Invoke(module);
    }

    public static void UseWebModule<TWebModule>(this WebApplication app)
        where TWebModule : class, IWebModule<TWebModule>, new()
    {
        TWebModule.Use(app);
        Log.ForContext<TWebModule>().Debug("Using WebModule: {Module}", typeof(TWebModule).Name);
    }
}
