using MongoDB.Driver.Linq;
using System.Runtime.CompilerServices;

namespace MongoDB.Driver;

public static class MongoDbMixins
{
    public static IMongoQueryable<TResult> ApplyQuery<T, TResult>(this IMongoQueryable<T> mongoQuery, Func<IQueryable<T>, IQueryable<TResult>> query)
    {
        return ((IMongoQueryable<TResult>)query(mongoQuery));
    }

    public static IMongoQueryable<TResult> ApplyQuery<T, TState, TResult>(this IMongoQueryable<T> mongoQuery, TState state, Func<IQueryable<T>, TState, IQueryable<TResult>> query)
    {
        return ((IMongoQueryable<TResult>)query(mongoQuery, state));
    }

    /// <summary>
    /// Executes the query and returns the results as a streamed async enumeration.
    /// </summary>
    /// <typeparam name="T">The type of the returned entities.</typeparam>
    /// <param name="source">The query source.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
    /// <returns>The streamed async enumeration containing the results.</returns>
    public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IAsyncCursorSource<T> source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        return ExecuteAsync(source, cancellationToken);

        static async IAsyncEnumerable<T> ExecuteAsync(IAsyncCursorSource<T> source, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var cursor = await source.ToCursorAsync(cancellationToken);

            while (await cursor.MoveNextAsync(cancellationToken))
            {
                foreach (var element in cursor.Current)
                {
                    yield return element;
                }
            }
        }
    }

    /// <summary>
    /// Executes the query and returns the results as a streamed async enumeration.
    /// </summary>
    /// <typeparam name="T">The type of the returned entities.</typeparam>
    /// <param name="source">The query source.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
    /// <returns>The streamed async enumeration containing the results.</returns>
    public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IMongoQueryable<T> source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        return ((IAsyncCursorSource<T>)source).ToAsyncEnumerable(cancellationToken);
    }

    /// <summary>
    /// Executes the cursor and returns the results as a streamed async enumeration.
    /// </summary>
    /// <typeparam name="T">The type of the returned entities.</typeparam>
    /// <param name="source">The cursor source.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
    /// <returns>The streamed async enumeration containing the results.</returns>
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IAsyncCursor<T> source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (await source.MoveNextAsync(cancellationToken))
        {
            foreach (var element in source.Current)
            {
                yield return element;
            }
        }
    }

    /// <summary>
    /// Executes the cursor and returns the results as a streamed async enumeration.
    /// </summary>
    /// <typeparam name="T">The type of the returned entities.</typeparam>
    /// <param name="source">The cursor source.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
    /// <returns>The streamed async enumeration containing the results.</returns>
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this Task<IAsyncCursor<T>> source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        var cursor = await source;
        await foreach (var item in cursor.ToAsyncEnumerable(cancellationToken))
        {
            yield return item;
        }
    }
}
