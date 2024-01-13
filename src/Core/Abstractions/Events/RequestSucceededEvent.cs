using Mediator;

namespace Core.Abstractions.Requests;

public class RequestEvent : IMessage
{
    private readonly Lazy<Dictionary<object, object?>> _items = new(() => []);

    /// <summary>
    /// The request.
    /// </summary>
    public IMessage Request { get; }

    /// <summary>
    /// Gets or sets a key/value collection that can be used to share data within the scope of this event.
    /// </summary>
    public IDictionary<object, object?> Items => _items.Value;

    public RequestEvent(IMessage request)
    {
        Request = request;
    }
}

/// <summary>
/// Event that is raised when a request succeeds.
/// </summary>
/// <remarks>These must be raised from a behavior or the user.</remarks>
public class RequestSucceededEvent : RequestEvent
{
    public RequestSucceededEvent(IMessage request) : base(request)
    {
    }
}

/// <summary>
/// Event that is raised when a request succeeds.
/// </summary>
/// <remarks>These must be raised from a behavior or the user.</remarks>
public class RequestSucceededEvent<TRequest> : RequestSucceededEvent where TRequest : IMessage
{
    /// <summary>
    /// The request which succeeded.
    /// </summary>
    public new TRequest Request => (TRequest)base.Request;

    /// <summary>
    /// Initializes a new instance of <see cref="RequestSucceededEvent"/> for the current request.
    /// </summary>
    /// <param name="request">The request this event is initialized for.</param>
    public RequestSucceededEvent(TRequest request) : base(request)
    {
    }
}
