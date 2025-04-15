using Blackwing.Contracts.Requests;

namespace Core.Abstractions.Requests;

/// <summary>
/// Event that is raised when a request fails.
/// </summary>
public class RequestFailedEvent : RequestEvent
{
    /// <summary>
    /// The problem this request generated.
    /// </summary>
    public Problem Problem { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="RequestFailedEvent"/> for the current request.
    /// </summary>
    /// <param name="request">The request this event is initialized for.</param>
    /// <param name="problem">The problem the request generated.</param>
    public RequestFailedEvent(IRequest request, Problem problem) : base(request)
    {
        Problem = problem;
    }
}

/// <summary>
/// Event that is raised when a request fails.
/// </summary>
public class RequestFailedEvent<TRequest> : RequestFailedEvent where TRequest : IRequest
{
    /// <summary>
    /// The request which failed.
    /// </summary>
    public new TRequest Request => (TRequest)base.Request;

    /// <summary>
    /// Initializes a new instance of <see cref="RequestFailedEvent"/> for the current request.
    /// </summary>
    /// <param name="request">The request this event is initialized for.</param>
    /// <param name="problem">The problem the request generated.</param>
    public RequestFailedEvent(IRequest request, Problem problem) : base(request, problem)
    {
    }
}
