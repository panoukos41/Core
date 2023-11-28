using Microsoft.AspNetCore.Http;

namespace Core.Commons;

public static class IResultUnionMixins
{
    /// <summary>
    /// Produces a <see cref="StatusCodes.Status200OK"/> response.
    /// </summary>
    public static ValueTask<IResult> ToOk<T>(this ValueTask<Result<T>> result)
        where T : notnull
        => result.MatchAsync(ok => Results.Ok(ok.Value), ToError);

    /// <summary>
    /// Produces a <see cref="StatusCodes.Status201Created"/> response.
    /// </summary>
    /// <param name="createdAtUri">Produces the URI at which the content has been created.</param>
    public static ValueTask<IResult> ToCreated<T>(this ValueTask<Result<T>> result, Func<T, string>? createdAtUri = null)
        where T : notnull
        => result.MatchAsync(ok => Results.Created(uri: createdAtUri?.Invoke(ok.Value), value: ok.Value), ToError);

    /// <summary>
    /// Produces a <see cref="StatusCodes.Status202Accepted"/> response.
    /// </summary>
    /// <param name="acceptedAt">Produces the URI with the location at which the status of requested content can be monitored.</param>
    public static ValueTask<IResult> ToAccepted<T>(this ValueTask<Result<T>> result, Func<T, string>? acceptedAt = null)
        where T : notnull
        => result.MatchAsync(ok => Results.Accepted(acceptedAt?.Invoke(ok.Value), ok.Value), ToError);

    /// <summary>
    /// Produces a <see cref="StatusCodes.Status204NoContent"/> response.
    /// </summary>
    public static ValueTask<IResult> ToNoContent<T>(this ValueTask<Result<T>> result)
        where T : notnull
        => result.MatchAsync(ok => Results.NoContent(), ToError);

    private static IResult ToError<T>(Result<T>.Er er)
        where T : notnull
        => er.Problem switch
        {
            { Status: 400 } => TypedResults.BadRequest(er.Problem),
            { Status: 401 } => TypedResults.Unauthorized(),
            { Status: 403 } => TypedResults.Forbid(),
            { Status: 404 } => TypedResults.NotFound(er.Problem),
            { Status: 409 } => TypedResults.Conflict(er.Problem),
            { Status: 422 } => TypedResults.UnprocessableEntity(er.Problem),
            _ => TypedResults.StatusCode(er.Problem.Status ?? StatusCodes.Status500InternalServerError)
        };
}
