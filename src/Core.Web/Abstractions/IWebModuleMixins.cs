using FluentValidation;
using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace Core.Abstractions;

public static class IWebModuleMixins
{
    private static readonly Type validType = typeof(IValid);
    private static readonly Dictionary<Type, object> modules = [];

    public static WebApplicationBuilder AddAppModule<TAppModule>(this WebApplicationBuilder builder, Action<TAppModule>? configure = null)
        where TAppModule : class, IAppModule<TAppModule>, new()
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        services.AddAppModule(configuration, configure);

        return builder;
    }

    public static WebApplicationBuilder AddWebModule<TWebModule>(this WebApplicationBuilder builder, Action<TWebModule>? configure = null)
        where TWebModule : class, IWebModule<TWebModule>, new()
    {
        var moduleType = typeof(TWebModule);
        var module = new TWebModule();

        if (modules.TryAdd(moduleType, module))
        {
            configure?.Invoke(module);
            if (moduleType.IsAssignableTo(validType))
            {
                var validatorProperty = moduleType.GetProperty(nameof(IValid.Validator), BindingFlags.Static | BindingFlags.Public)!;
                var validator = (IValidator)validatorProperty.GetValue(null)!;
                var context = new ValidationContext<object>(module);
                var result = validator.Validate(context);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => $"{x.PropertyName}: {x.ErrorMessage} Severity: {x.Severity}");
                    throw new ValidationException(result.Errors);
                }
            }
            TWebModule.Add(builder, module);
            Log.ForContext<TWebModule>().Debug("Added WebModule: {Module}", moduleType.Name);
        }
        return builder;
    }

    public static WebApplication UseWebModule<TWebModule>(this WebApplication app)
        where TWebModule : class, IWebModule<TWebModule>, new()
    {
        var moduleType = typeof(TWebModule);
        if (modules.TryGetValue(moduleType, out var module))
        {
            TWebModule.Use(app, (TWebModule)module);
            Log.ForContext<TWebModule>().Debug("Using WebModule: {Module}", typeof(TWebModule).Name);
        }
        else
        {
            throw new InvalidOperationException($"Module {moduleType} was not added. First add it and then use it.");
        }
        return app;
    }
}
