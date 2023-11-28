using Core.MongoDb.Commons.BsonSerializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public class CoreMongoDbAppModule : IAppModule<CoreMongoDbAppModule>
{
    public static void Add(IServiceCollection services, IConfiguration configuration, CoreMongoDbAppModule module)
    {
        PhoneBsonSerializer.RegisterProvider();
        UuidBsonSerializer.RegisterProvider();
    }
}
