using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Core.MongoDb.Common.BsonSerializers;

public sealed class JsonObjectSerializer :
    SerializerBase<JsonObject>,
    IBsonSerializationProvider
{
    private static readonly Type type = typeof(JsonObject);

    private static JsonObjectSerializer Provider { get; } = new();

    // todo: Look into IBsonDocumentSerializer

    public static void RegisterProvider()
    {
        BsonSerializer.RegisterSerializationProvider(Provider);
    }

    public IBsonSerializer GetSerializer(Type type)
    {
        return type == JsonObjectSerializer.type ? Provider : null!;
    }

    public override JsonObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bson = BsonDocumentSerializer.Instance.Deserialize(context, args);
        if (bson.ElementCount is 0)
        {
            return [];
        }

        var json = bson.ToJson();
        return JsonSerializer.Deserialize<JsonObject>(json) ?? [];
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JsonObject value)
    {
        BsonDocumentSerializer.Instance.Serialize(context, args, BsonDocument.Parse(value.ToJsonString()));
    }
}