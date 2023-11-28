using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core;

public static class IAppModuleMixins
{
    public static void AddAppModule<TAppModule>(this IServiceCollection services, IConfiguration configuration, Action<TAppModule>? configure = null)
        where TAppModule : class, IAppModule<TAppModule>, new()
    {
        var count = services.Count;
        var module = new TAppModule();
        services.TryAddSingleton(module);

        if (count == services.Count) return;

        configure?.Invoke(module);
        TAppModule.Add(services, configuration, module);

        Log.ForContext<TAppModule>().Debug("Added AppModule: {AppModule}", typeof(TAppModule).Name);
    }

    public static void ValidateAndThrow<TAppModule>(this TAppModule module)
        where TAppModule : class, IAppModule<TAppModule>, IValid, new()
    {
        var ctx = new ValidationContext<TAppModule>(module);
        var result = TAppModule.Validator.Validate(ctx);

        if (result.Errors is { Count: > 0 } errors)
        {
            var type = module.GetType().Name;
            throw new ValidationException($"Add {type}", errors, true);
        }
    }
}
