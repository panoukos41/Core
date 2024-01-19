using Core.Abstractions;
using Core.MongoDb.Common.BsonSerializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public class CoreMongoDbAppModule : IAppModule<CoreMongoDbAppModule>
{
    public static void Add(IServiceCollection services, IConfiguration configuration)
    {
        JsonObjectBsonSerializer.RegisterProvider();
        PhoneBsonSerializer.RegisterProvider();
        UuidBsonSerializer.RegisterProvider();
    }
}
