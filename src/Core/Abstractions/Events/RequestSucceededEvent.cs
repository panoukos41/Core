using Blackwing.Contracts.Requests;

namespace Core.Abstractions.Requests;

/// <summary>
/// Event that is raised when a request succeeds.
/// </summary>
/// <remarks>These must be raised from a behavior or the user.</remarks>
public class RequestSucceededEvent : RequestEvent
{
    public RequestSucceededEvent(IRequest request) : base(request)
    {
    }
}

/// <summary>
/// Event that is raised when a request succeeds.
/// </summary>
/// <remarks>These must be raised from a behavior or the user.</remarks>
public class RequestSucceededEvent<TRequest> : RequestSucceededEvent where TRequest : IRequest
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
