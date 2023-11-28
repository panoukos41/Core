using Core.Commons;

namespace MongoDB.Driver;

public static class FilterMixins
{
    /// <summary>
    /// Adds an And filter that uses an Eq operation for the provided <paramref name="concurrencyStamp"/>.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="filter">The filter to And to.</param>
    /// <param name="concurrencyStamp">The concurrency stamp to use.</param>
    /// <returns>The And filter definition.</returns>
    public static FilterDefinition<TDocument> AndConcurrencyStamp<TDocument>(this FilterDefinition<TDocument> filter, string concurrencyStamp)
        where TDocument : IConcurrencyStamp
    {
        return Builders<TDocument>.Filter.And(
            Builders<TDocument>.Filter.Eq(x => x.ConcurrencyStamp, concurrencyStamp),
            filter
        );
    }

    /// <summary>
    /// Adds an And filter that uses an Eq operation for the provided
    /// <see cref="IConcurrencyStamp.ConcurrencyStamp"/> of the <paramref name="document"/>.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="filter">The filter to And to.</param>
    /// <param name="document">The document whose concurrency stamp to use.</param>
    /// <returns>The And filter definition.</returns>
    public static FilterDefinition<TDocument> AndConcurrencyStamp<TDocument>(this FilterDefinition<TDocument> filter, TDocument document)
        where TDocument : IConcurrencyStamp
    {
        return filter.AndConcurrencyStamp(document.ConcurrencyStamp);
    }
}
