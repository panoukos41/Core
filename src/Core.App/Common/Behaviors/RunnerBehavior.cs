using Core.Abstractions.Requests;

namespace Core.Common.Behaviors;

public sealed class RunnerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
    where TResponse : IResultUnion
{
    private readonly IEventPublisher eventPublisher;

    public RunnerBehavior(IEventPublisher eventPublisher)
    {
        this.eventPublisher = eventPublisher;
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
        var @event = new RequestSucceededEvent<TRequest>(request);
        eventPublisher.Publish(@event);
    }

    private void PublishRequestFailed(TRequest request, Problem problem)
    {
        var @event = new RequestFailedEvent<TRequest>(request, problem);
        eventPublisher.Publish(@event);
    }
}
