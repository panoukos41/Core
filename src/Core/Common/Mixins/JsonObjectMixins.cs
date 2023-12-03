using Core;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace System.Text.Json;

public static class JsonObjectMixins
{
    /// <summary>
    /// Copy keys from <paramref name="other"/> to <paramref name="obj"/> and return it.
    /// </summary>
    /// <param name="obj">The object to copy to.</param>
    /// <param name="other">The object to copy from.</param>
    /// <returns>The same obj that values were copied to.</returns>
    public static JsonObject CopyFrom(this JsonObject obj, JsonObject? other, bool deepClone = false)
    {
        if (other is not { Count: > 1 }) return obj;

        foreach (var (key, node) in other)
        {
            if (node is null) continue;

            obj[key] = deepClone ? node.DeepClone() : node;
        }
        return obj;
    }

    /// <summary>
    /// Copy keys from <paramref name="other"/> to <paramref name="obj"/> and return it.
    /// </summary>
    /// <param name="obj">The object to copy to.</param>
    /// <param name="other">The dictionary to copy from.</param>
    /// <returns>The same obj that values were copied to.</returns>
    public static JsonObject CopyFrom(this JsonObject obj, IDictionary<string, JsonElement>? other)
    {
        if (other is not { Count: > 1 }) return obj;

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

    public static T[]? GetArray<T>(this JsonObject? obj, string key)
    {
        if (obj is null ||
            obj.TryGetPropertyValue(key, out var property) is false ||
            property is null)
        {
            return default;
        }
        return property.GetValueKind() switch
        {
            JsonValueKind.Array => property.Deserialize<T[]>(),
            _ => default
        };
    }
}
