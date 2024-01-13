using Core.Abstractions;
using Core.Common.Hosting;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

    public static void Use(WebApplication app)
    {
        var modules = Enumerable.Concat(
            IAppModuleMixins.Modules.Values.Where(x => x is IValid valid),
            IWebModuleMixins.Modules.Values.Where(x => x is IValid valid)
        );

        foreach (var module in modules)
        {
            var moduleType = module.GetType();
            var validatorProperty = moduleType.GetProperty(nameof(IValid.Validator), BindingFlags.Static | BindingFlags.Public)!;
            var validator = (IValidator)validatorProperty.GetValue(null)!;
            var context = new ValidationContext<object>(module);

            if (validator.Validate(context).Errors is { Count: > 0 } errors)
            {
                throw new ValidationException($"{moduleType}", errors, true);
            }
        }
    }
}
