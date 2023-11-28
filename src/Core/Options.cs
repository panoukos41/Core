using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core;

public static class Options
{
    public static JsonSerializerOptions Json { get; set; } = new()
    {
        PropertyNamingPolicy = null,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Converters = { new ResultJsonConverter() }
    };

    public static void Merge(this JsonSerializerOptions options, JsonSerializerOptions other)
    {
        options.PropertyNamingPolicy = other.PropertyNamingPolicy;
        options.PropertyNameCaseInsensitive = other.PropertyNameCaseInsensitive;
        options.DefaultIgnoreCondition = other.DefaultIgnoreCondition;
        options.ReferenceHandler = other.ReferenceHandler;
        foreach (var converter in other.Converters.Except(options.Converters))
        {
            options.Converters.Add(converter);
        }
    }

    [ModuleInitializer]
    [SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute should not be used in libraries", Justification = "<Pending>")]
    internal static void Initializer()
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
    }
}
