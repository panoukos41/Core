using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Core.MongoDb.Commons.BsonSerializers;

public sealed class UuidBsonSerializer : SerializerBase<Uuid>, IBsonSerializationProvider
{
    private static readonly Type type = typeof(Uuid);

    private static UuidBsonSerializer Provider { get; } = new();

    public static void RegisterProvider()
    {
        BsonSerializer.RegisterSerializationProvider(Provider);
    }

    public IBsonSerializer GetSerializer(Type type)
    {
        return type == UuidBsonSerializer.type ? Provider : null!;
    }

    public override Uuid Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return Uuid.TryParse(context.Reader.ReadString(), out var uuid) ? uuid : Uuid.Empty;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Uuid value)
    {
        context.Writer.WriteString(value.ToString());
    }
}
