using Core.Abstractions.Events;
using Core.Abstractions.Requests;

namespace Core.Commons.Behaviors;

public sealed class RunnerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
    where TResponse : IResultUnion
{
    private readonly IEventHandler<RequestSucceeded>[] successEventHandlers;
    private readonly IEventHandler<RequestFailed>[] failedEventHandlers;

    public RunnerBehavior(
        IEnumerable<IEventHandler<RequestSucceeded>> successEventHandlers,
        IEnumerable<IEventHandler<RequestFailed>> failedEventHandlers)
    {
        this.successEventHandlers = [.. successEventHandlers];
        this.failedEventHandlers = [.. failedEventHandlers];
    }

    public async ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        try
        {
            var result = await next(request, cancellationToken);
            if (result.IsEr(out var problem))
            {
                Error(request, problem);
            }
            PublishRequestSuccess(request);
            return result;
        }
        catch (Exception ex)
        {
            Error(request, ex);
            return (TResponse)TResponse.CreateEr(ex);
        }
    }

    private void Error(TRequest request, Problem problem)
    {
        var logger = Log.ForContext<TRequest>();
        var template = request switch
        {
            IBaseQuery => "Query {Name}, {Problem}",
            IBaseCommand => "Command {Name}, {Problem}",
            _ => "RQST {Name}, {Problem}"
        };
        logger.Error(template, request.GetType().Name, problem);
        PublishRequestFailed(request);
    }

    private void PublishRequestSuccess(TRequest request)
    {
        var @event = new RequestSucceeded
        {
            Request = request,
            Context = GetRequestContext()
        };
        foreach (var h in successEventHandlers)
        {
            // todo: Add logger on exception.
            h.Handle(@event, default).FireAndForget();
        }
    }

    private void PublishRequestFailed(TRequest request)
    {
        var @event = new RequestFailed
        {
            Request = request,
            Context = GetRequestContext()
        };
        foreach (var h in failedEventHandlers)
        {
            // todo: Add logger on exception.
            h.Handle(@event, default).FireAndForget();
        }
    }

    private RequestContext GetRequestContext() => new()
    {
        Items = new Dictionary<object, object?>(),
        User = new UserPrincipal()
    };
}
