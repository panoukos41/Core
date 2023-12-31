using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Search;

namespace Core.MongoDb.Common;

public abstract class MongoDbContext
{
    public MongoClient Client { get; }

    public IMongoDatabase Database { get; }

    public MongoCollectionSettings? CollectionSettings { get; protected set; }

    protected MongoDbContext(MongoClient client, string database, MongoCollectionSettings? collectionSettings = null)
    {
        Client = client;
        Database = client.GetDatabase(database);
        CollectionSettings = collectionSettings;
    }

    public MongoCollectionContext<TDocument> GetCollection<TDocument>(string? collection = null, MongoCollectionSettings? options = null)
    {
        return new(Database.GetCollection<TDocument>(collection, options ?? CollectionSettings));
    }
}

public sealed class MongoCollectionContext<TDocument> : MongoCollectionBase<TDocument>
{
    private readonly IMongoCollection<TDocument> collection;

    public MongoCollectionContext(IMongoCollection<TDocument> collection)
    {
        this.collection = collection;
    }

    #region Builders

    public FilterDefinitionBuilder<TDocument> Filter
        => Builders<TDocument>.Filter;

    public IndexKeysDefinitionBuilder<TDocument> IndexKeys
        => Builders<TDocument>.IndexKeys;

    public ProjectionDefinitionBuilder<TDocument> Projection
        => Builders<TDocument>.Projection;

    public SetFieldDefinitionsBuilder<TDocument> SetFields
        => Builders<TDocument>.SetFields;

    public SortDefinitionBuilder<TDocument> Sort
        => Builders<TDocument>.Sort;

    public UpdateDefinitionBuilder<TDocument> Update
        => Builders<TDocument>.Update;

    public SearchFacetBuilder<TDocument> SearchFacet
        => Builders<TDocument>.SearchFacet;

    public SearchPathDefinitionBuilder<TDocument> SearchPath
        => Builders<TDocument>.SearchPath;

    public SearchScoreDefinitionBuilder<TDocument> SearchScore
        => Builders<TDocument>.SearchScore;

    public SearchScoreFunctionBuilder<TDocument> SearchScoreFunction
        => Builders<TDocument>.SearchScoreFunction;

    public SearchDefinitionBuilder<TDocument> Search
        => Builders<TDocument>.Search;

    public SearchSpanDefinitionBuilder<TDocument> SearchSpan
        => Builders<TDocument>.SearchSpan;

    #endregion

    #region MongoCollectionBase

    public override CollectionNamespace CollectionNamespace => collection.CollectionNamespace;

    public override IMongoDatabase Database => collection.Database;

    public override IBsonSerializer<TDocument> DocumentSerializer => collection.DocumentSerializer;

    public override IMongoIndexManager<TDocument> Indexes => collection.Indexes;

    public override IMongoSearchIndexManager SearchIndexes => collection.SearchIndexes;

    public override MongoCollectionSettings Settings => collection.Settings;

    public override Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions? options = null, CancellationToken cancellationToken = default)
    {
        return collection.AggregateAsync(pipeline, options, cancellationToken);
    }

    public override Task<BulkWriteResult<TDocument>> BulkWriteAsync(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions? options = null, CancellationToken cancellationToken = default)
    {
        return collection.BulkWriteAsync(requests, options, cancellationToken);
    }

    [Obsolete("Use CountDocuments or EstimatedDocumentCount instead.")]
    public override Task<long> CountAsync(FilterDefinition<TDocument> filter, CountOptions? options = null, CancellationToken cancellationToken = default)
    {
        return collection.CountAsync(filter, options, cancellationToken);
    }

    public override Task<IAsyncCursor<TField>> DistinctAsync<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions? options = null, CancellationToken cancellationToken = default)
    {
        return collection.DistinctAsync(field, filter, options, cancellationToken);
    }

    public override Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection>? options = null, CancellationToken cancellationToken = default)
    {
        return collection.FindAsync(filter, options, cancellationToken);
    }

    public override Task<TProjection> FindOneAndDeleteAsync<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection>? options = null, CancellationToken cancellationToken = default)
    {
        return collection.FindOneAndDeleteAsync(filter, options, cancellationToken);
    }

    public override Task<TProjection> FindOneAndReplaceAsync<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection>? options = null, CancellationToken cancellationToken = default)
    {
        return collection.FindOneAndReplaceAsync(filter, replacement, options, cancellationToken);
    }

    public override Task<TProjection> FindOneAndUpdateAsync<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection>? options = null, CancellationToken cancellationToken = default)
    {
        return collection.FindOneAndUpdateAsync(filter, update, options, cancellationToken);
    }

    [Obsolete("Use Aggregation pipeline instead.")]
    public override Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult>? options = null, CancellationToken cancellationToken = default)
    {
        return collection.MapReduceAsync(map, reduce, options, cancellationToken);
    }

    public override IFilteredMongoCollection<TDerivedDocument> OfType<TDerivedDocument>()
    {
        return collection.OfType<TDerivedDocument>();
    }

    public override IMongoCollection<TDocument> WithReadPreference(ReadPreference? readPreference)
    {
        return collection.WithReadPreference(readPreference);
    }

    public override IMongoCollection<TDocument> WithWriteConcern(WriteConcern? writeConcern)
    {
        return collection.WithWriteConcern(writeConcern);
    }

    #endregion
}
