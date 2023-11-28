using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Core.Commons;

public class SwaggerWebModule : SwaggerUIOptions, IWebModule<SwaggerWebModule>
{
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

    public static void Use(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var module = app.Services.GetRequiredService<SwaggerWebModule>();

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
