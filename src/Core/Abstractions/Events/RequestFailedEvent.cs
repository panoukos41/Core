using Mediator;

namespace Core.Abstractions.Requests;

/// <summary>
/// Event that is raised when a request fails.
/// </summary>
public class RequestFailedEvent
{
    /// <summary>
    /// The request which failed.
    /// </summary>
    public IMessage Request { get; }

    /// <summary>
    /// The problem this request generated.
    /// </summary>
    public Problem Problem { get; }

    /// <summary>
    /// Gets or sets a key/value collection that can be used to share data within the scope of this event.
    /// </summary>
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();

    /// <summary>
    /// Initializes a new instance of <see cref="RequestSucceededEvent"/> for the current request.
    /// </summary>
    /// <param name="request">The request this event is initialized for.</param>
    /// <param name="problem">The problem the request generated.</param>
    public RequestFailedEvent(IMessage request, Problem problem)
    {
        Request = request;
        Problem = problem;
    }
}

/// <summary>
/// Event that is raised when a request fails.
/// </summary>
public class RequestFailedEvent<TRequest> : RequestFailedEvent where TRequest : IMessage
{
    /// <summary>
    /// The request which failed.
    /// </summary>
    public new TRequest Request => (TRequest)Request;

    /// <summary>
    /// Initializes a new instance of <see cref="RequestSucceededEvent"/> for the current request.
    /// </summary>
    /// <param name="request">The request this event is initialized for.</param>
    /// <param name="problem">The problem the request generated.</param>
    public RequestFailedEvent(IMessage request, Problem problem) : base(request, problem)
    {
    }
}
