using Core.Commons;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Core.MongoDb.Commons.BsonSerializers;

public sealed class PhoneBsonSerializer : SerializerBase<Phone>, IBsonSerializationProvider
{
    private static readonly Type type = typeof(Phone);

    private static PhoneBsonSerializer Provider { get; } = new();

    // todo: Look into IBsonDocumentSerializer

    public static void RegisterProvider()
    {
        BsonSerializer.RegisterSerializationProvider(Provider);
    }

    public IBsonSerializer GetSerializer(Type type)
    {
        return type == PhoneBsonSerializer.type ? Provider : null!;
    }

    public override Phone Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return Phone.TryParse(context.Reader.ReadString(), out var phone) ? phone : Phone.Empty;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Phone value)
    {
        context.Writer.WriteString(value.ToString());
    }
}
