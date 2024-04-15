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
        //ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Converters = { new ResultJsonConverter() }
    };

    public static void Apply(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = Json.PropertyNamingPolicy;
        options.PropertyNameCaseInsensitive = Json.PropertyNameCaseInsensitive;
        options.DefaultIgnoreCondition = Json.DefaultIgnoreCondition;
        options.ReferenceHandler = Json.ReferenceHandler;
        options.PreferredObjectCreationHandling = Json.PreferredObjectCreationHandling;
        foreach (var converter in Json.Converters.Except(options.Converters))
        {
            options.Converters.Add(converter);
        }
    }

    public static void Replace(ref JsonSerializerOptions options)
    {
        options = Json;
    }
}
