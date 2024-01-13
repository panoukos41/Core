using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Core.Abstractions;

public static class IAppModuleMixins
{
    private static readonly Dictionary<Type, object> modules = [];

    public static IReadOnlyDictionary<Type, object> Modules => new ReadOnlyDictionary<Type, object>(modules);

    public static void AddAppModule<TAppModule>(this IServiceCollection services, IConfiguration configuration, Action<TAppModule>? configure = null)
        where TAppModule : class, IAppModule<TAppModule>, new()
    {
        var moduleType = typeof(TAppModule);
        TAppModule module;
        if (modules.TryGetValue(moduleType, out var value))
        {
            module = (TAppModule)value;
        }
        else
        {
            module = new TAppModule();
            modules.Add(moduleType, module);
            services.AddSingleton(module);
            TAppModule.Add(services, configuration);
            Log.ForContext<TAppModule>().Debug("Added AppModule: {Module}", moduleType.Name);
        }
        configure?.Invoke(module);
    }
}
