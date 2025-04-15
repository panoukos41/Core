using Blackwing.Contracts.Requests;

namespace Core.Common.Behaviors;

public sealed class RunnerBehavior<TRequest, TResponse> : IRequestPipeline<TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : IResultUnion
{
    public async ValueTask<TResponse> Handle(TRequest request, IRequestPipelineDelegate<TRequest, TResponse> next, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await next(request, cancellationToken);
            if (result.IsEr(out var problem))
            {
                RunnerBehavior<TRequest, TResponse>.Error(request, problem);
            }
            return result;
        }
        catch (Exception ex)
        {
            RunnerBehavior<TRequest, TResponse>.Error(request, ex);
            return (TResponse)TResponse.CreateEr(ex);
        }
    }

    private static void Error(TRequest request, Problem problem)
    {
        var logger = Log.ForContext<TRequest>();
        logger.Error(problem.Exception, "Problem for RQST {Name}, {Type} {Title} {Status} {Detail}", request.GetType().Name, problem.Type, problem.Title, problem.Status, problem.Detail);
    }
}
