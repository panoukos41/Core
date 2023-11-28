using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Commons;

public class LoggingWebModule : LoggerConfiguration, IWebModule<LoggingWebModule>
{
    /// <summary>
    /// Whether to add default values or not. Defaults to true.
    /// </summary>
    /// <remarks>
    /// WriteTo: <b>Console</b> (always) and <b>File</b> (release). <br/>
    /// MinimumLevel = Information when debug and Warning when release.
    /// </remarks>
    public bool AddDefaults { get; set; } = true;

    /// <summary>
    /// Sets <see cref="AddDefaults"/> to <see langword="false"/> in a fluent manner.
    /// </summary>
    public LoggingWebModule NoDefaults()
    {
        AddDefaults = false;
        return this;
    }

    public static void Add(WebApplicationBuilder builder, LoggingWebModule module)
    {
        if (module.AddDefaults)
        {
            module
                .WriteTo.Console()
#if DEBUG
                .MinimumLevel.Information();
#else
                .WriteTo.File($"{Environment.CurrentDirectory}/Logs/logs.txt")
                .MinimumLevel.Warning();
#endif
        }

        Log.Logger = module.CreateLogger();
        builder.Host.UseSerilog();
    }

    public static void Use(WebApplication app)
    {
        app.Services.GetRequiredService<LoggingWebModule>(); // check module has been registered.

        app.UseSerilogRequestLogging();
    }
}
