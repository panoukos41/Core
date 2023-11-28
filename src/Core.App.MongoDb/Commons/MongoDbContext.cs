using MongoDB.Driver;

namespace Core.MongoDb.Commons;

public abstract class MongoDbContext
{
    public abstract MongoClient Client { get; }

    public abstract IMongoDatabase Database { get; }

    public MongoCollectionSettings? CollectionSettings { get; protected set; }

    public IMongoCollection<TDocument> GetCollection<TDocument>(string? collection = null, MongoCollectionSettings? options = null)
    {
        return Database.GetCollection<TDocument>(collection, options ?? CollectionSettings);
    }
}
