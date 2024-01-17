using Core.Abstractions;
using Core.Primitives;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Core.Common.Modules;

public class SwaggerWebModule : SwaggerUIOptions, IWebModule<SwaggerWebModule>
{
    public bool Enable { get; set; }

    public static void Add(WebApplicationBuilder builder, SwaggerWebModule module)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => { });
        services.ConfigureSwaggerGen(options =>
        {
            options.SupportNonNullableReferenceTypes();
            options.MapType<Uuid>(() => new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(string.Empty)
            });
            options.MapType<Phone>(() => new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(string.Empty)
            });
        });
    }

    public static void Use(WebApplication app, SwaggerWebModule module)
    {
        if (app.Environment.IsDevelopment() || module.Enable)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {

                options.ConfigObject = module.ConfigObject;
                options.DocumentTitle = module.DocumentTitle;
                options.HeadContent = module.HeadContent;
                options.IndexStream = module.IndexStream;
                options.Interceptors = module.Interceptors;
                options.OAuthConfigObject = module.OAuthConfigObject;
                options.RoutePrefix = module.RoutePrefix == "swagger" ? "docs" : module.RoutePrefix;
            });
            app.MapSwagger();
        }
    }
}
