using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Core.Abstractions;

public static class IAppModuleMixins
{
    private static readonly Type validType = typeof(IValid);
    private static readonly HashSet<Type> modules = [];

    public static void AddAppModule<TAppModule>(this IServiceCollection services, IConfiguration configuration, Action<TAppModule>? configure = null)
        where TAppModule : class, IAppModule<TAppModule>, new()
    {
        var moduleType = typeof(TAppModule);
        if (modules.Add(moduleType))
        {
            services.AddScoped(sp => sp.GetRequiredService<IOptionsMonitor<TAppModule>>().CurrentValue);
            if (moduleType.IsAssignableTo(validType))
            {
                services.AddSingleton<IValidateOptions<TAppModule>, CustomValidateOptions<TAppModule>>();
            }
            TAppModule.Add(services, configuration);
            Log.ForContext<TAppModule>().Debug("Added AppModule: {Module}", moduleType.Name);
        }
        if (configure is { })
        {
            services.Configure(configure);
        }
    }

    private sealed class CustomValidateOptions<TAppModule> : IValidateOptions<TAppModule> where TAppModule : class
    {
        private static readonly Lazy<IValidator> Validator = new(static () =>
        {
            var moduleType = typeof(TAppModule);
            var validatorProperty = moduleType.GetProperty(nameof(IValid.Validator), BindingFlags.Static | BindingFlags.Public)!;
            return (IValidator)validatorProperty.GetValue(null)!;
        });

        public ValidateOptionsResult Validate(string? name, TAppModule module)
        {
            var validator = Validator.Value;
            var context = new ValidationContext<TAppModule>(module);

            var result = validator.Validate(context);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => $"{x.PropertyName}: {x.ErrorMessage} Severity: {x.Severity}");
                var ex = new ValidationException(result.Errors);
                Log.ForContext<TAppModule>().Fatal(ex, "Invalid module options {module}", module.GetType());
                return ValidateOptionsResult.Fail(errors);
            }
            return ValidateOptionsResult.Success;
        }
    }
}
