using Core;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace System.Text.Json;

public static class JsonObjectMixins
{
    public static JsonObject CopyFrom(this JsonObject obj, JsonObject other)
    {
        foreach (var (key, node) in other)
        {
            obj[key] = node;
        }
        return obj;
    }

    public static JsonObject CopyFrom(this JsonObject obj, IDictionary<string, JsonElement> other)
    {
        foreach (var (key, element) in other)
        {
            obj[key] = element.ValueKind switch
            {
                JsonValueKind.Object => JsonObject.Create(element),
                JsonValueKind.Array => JsonArray.Create(element),
                _ => JsonValue.Create(element)
            };
        }
        return obj;
    }

    [return: NotNullIfNotNull(nameof(obj))]
    public static ImmutableDictionary<string, JsonElement>? ToImmutableDictionary(this JsonObject? obj)
    {
        return obj?.ToImmutableDictionary(x => x.Key, x => JsonSerializer.Deserialize<JsonElement>(x.Value));
    }

    [return: NotNullIfNotNull(nameof(obj))]
    public static JsonObject? Upsert<T>(this JsonObject? obj, string key, T? value)
    {
        if (obj is null) return obj;

        if (value is null)
            obj.Remove(key);
        else
            obj[key] = JsonSerializer.SerializeToNode(value, Options.Json);

        return obj;
    }
}
