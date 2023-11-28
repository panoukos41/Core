using Mediator;

namespace Core.Abstractions.Requests;

/// <summary>
/// Event that is raised when a request succeeds.
/// </summary>
/// <remarks>These must be raised from a behavior or the user.</remarks>
public class RequestSucceededEvent
{
    /// <summary>
    /// The request which succeeded.
    /// </summary>
    public IMessage Request { get; }

    /// <summary>
    /// Gets or sets a key/value collection that can be used to share data within the scope of this event.
    /// </summary>
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();

    /// <summary>
    /// Initializes a new instance of <see cref="RequestSucceededEvent"/> for the current request.
    /// </summary>
    /// <param name="request">The request this event is initialized for.</param>
    public RequestSucceededEvent(IMessage request)
    {
        Request = request;
    }
}
