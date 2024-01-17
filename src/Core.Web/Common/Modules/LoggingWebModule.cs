using Core.Abstractions;
using Microsoft.AspNetCore.Builder;

namespace Core.Common.Modules;

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

    public static void Use(WebApplication app, LoggingWebModule module)
    {
        app.UseSerilogRequestLogging();
    }
}
