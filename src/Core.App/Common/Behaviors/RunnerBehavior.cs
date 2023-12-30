using Core.Abstractions.Events;
using Core.Abstractions.Requests;

namespace Core.Common.Behaviors;

public sealed class RunnerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
    where TResponse : IResultUnion
{
    private readonly IEnumerable<IEventHandler<RequestSucceededEvent>> successEventHandlers;
    private readonly IEnumerable<IEventHandler<RequestFailedEvent>> failedEventHandlers;

    public RunnerBehavior(
        IEnumerable<IEventHandler<RequestSucceededEvent>> successEventHandlers,
        IEnumerable<IEventHandler<RequestFailedEvent>> failedEventHandlers)
    {
        this.successEventHandlers = successEventHandlers;
        this.failedEventHandlers = failedEventHandlers;
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
            IBaseQuery => "Problem for Query {Name}, {Type} {Title} {Status} {Detail}",
            IBaseCommand => "Problem for Command {Name}, {Type} {Title} {Status} {Detail}",
            _ => "Problem for RQST {Name}, {Type} {Title} {Status} {Detail}"
        };
        logger.Error(problem.Exception, template, request.GetType().Name, problem.Type, problem.Title, problem.Status, problem.Detail);
        PublishRequestFailed(request, problem);
    }

    private void PublishRequestSuccess(TRequest request)
    {
        var @event = new RequestSucceededEvent(request);
        foreach (var h in successEventHandlers)
        {
            h.Handle(@event, default).FireAndForget(static ex => Log.ForContext<RequestSucceededEvent>().Error(ex, "Error on EventHandler"));
        }
    }

    private void PublishRequestFailed(TRequest request, Problem problem)
    {
        var @event = new RequestFailedEvent(request, problem);
        foreach (var h in failedEventHandlers)
        {
            h.Handle(@event, default).FireAndForget(static ex => Log.ForContext<RequestFailedEvent>().Error(ex, "Error on EventHandler"));
        }
    }
}
