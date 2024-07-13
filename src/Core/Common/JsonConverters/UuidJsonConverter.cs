using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Common.JsonConverters;

public sealed class UuidJsonConverter : JsonConverter<Uuid>
{
    public override Uuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Uuid.TryParse(reader.GetString(), out var uuid) ? uuid : Uuid.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Uuid value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(['"', .. value.ToString(), '"']);
    }

    public override Uuid ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Read(ref reader, typeToConvert, options);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] Uuid value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.ToString());
    }
}
