using Core.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core;

public static class IWebModuleMixins
{
    public static void AddAppModule<TAppModule>(this WebApplicationBuilder builder, Action<TAppModule>? configure = null)
        where TAppModule : class, IAppModule<TAppModule>, new()
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var count = services.Count;
        var module = new TAppModule();
        services.TryAddSingleton(module);

        if (count == services.Count) return;

        configure?.Invoke(module);
        TAppModule.Add(services, configuration, module);

        Log.ForContext<TAppModule>().Debug("Added AppModule: {AppModule}", typeof(TAppModule).Name);
    }

    public static void AddWebModule<TWebModule>(this WebApplicationBuilder builder, Action<TWebModule>? configure = null)
        where TWebModule : class, IWebModule<TWebModule>, new()
    {
        var count = builder.Services.Count;
        var module = new TWebModule();
        builder.Services.TryAddSingleton(module);

        if (count == builder.Services.Count) return;

        configure?.Invoke(module);
        TWebModule.Add(builder, module);

        Log.ForContext<TWebModule>().Debug("Added WebModule: {Module}", module.GetType().Name);
    }

    public static void UseWebModule<TWebModule>(this WebApplication app)
        where TWebModule : class, IWebModule<TWebModule>, new()
    {
        TWebModule.Use(app);
        Log.ForContext<TWebModule>().Debug("Using WebModule: {Module}", typeof(TWebModule).Name);
    }

    public static void ValidateAndThrow<TWebModule>(this TWebModule module)
        where TWebModule : class, IWebModule<TWebModule>, IValid, new()
    {
        var ctx = new ValidationContext<TWebModule>(module);
        var result = TWebModule.Validator.Validate(ctx);

        if (result.Errors is { Count: > 0 } errors)
        {
            var type = module.GetType().Name;
            throw new ValidationException($"Add {type}", errors, true);
        }
    }
}
