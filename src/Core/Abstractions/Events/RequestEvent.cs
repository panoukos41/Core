using Blackwing.Contracts.Requests;

namespace Core.Abstractions.Requests;

public class RequestEvent
{
    private readonly Lazy<Dictionary<object, object?>> _items = new(() => []);

    /// <summary>
    /// The request.
    /// </summary>
    public IRequest Request { get; }

    /// <summary>
    /// Gets or sets a key/value collection that can be used to share data within the scope of this event.
    /// </summary>
    public IDictionary<object, object?> Items => _items.Value;

    public RequestEvent(IRequest request)
    {
        Request = request;
    }
}
