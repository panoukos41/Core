using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Commons.JsonConverters;

public sealed class PhoneJsonConverter : JsonConverter<Phone>
{
    // todo: implement  IUtf8SpanParsable<Phone>, 
    public override Phone Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Phone.TryParse(reader.GetString(), out var phone)
            ? phone
            : Phone.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Phone value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
