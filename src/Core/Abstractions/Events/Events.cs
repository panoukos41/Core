using Core.Commons;
using Mediator;

namespace Core.Abstractions.Requests;

public sealed class RequestContext
{
    /// <summary>
    /// Gets or sets the user for this request.
    /// </summary>
    public required UserPrincipal? User { get; init; }

    /// <summary>
    /// Gets or sets a key/value collection that can be used to share data within the scope of this request.
    /// </summary>
    public required IDictionary<object, object?> Items { get; init; }
}

public class RequestSucceeded
{
    public required RequestContext Context { get; init; }

    public required IMessage Request { get; init; }
}


public class RequestFailed
{
    public required RequestContext Context { get; init; }

    public required IMessage Request { get; init; }
}
