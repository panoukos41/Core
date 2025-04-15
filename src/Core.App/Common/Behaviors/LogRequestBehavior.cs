using Blackwing.Contracts.Requests;
using Serilog.Events;

namespace Core.Common.Behaviors;

public sealed class LogRequestBehavior<TRequest, TResponse> : IRequestPipeline<TRequest, TResponse>
    where TRequest : IRequest
{
    public ValueTask<TResponse> Handle(TRequest request, IRequestPipelineDelegate<TRequest, TResponse> next, CancellationToken cancellationToken = default)
    {
        var logger = Log.ForContext<TRequest>();

        if (!logger.IsEnabled(LogEventLevel.Information))
            return next(request, cancellationToken);

        logger.Information("RQST {Request}", request.GetType().Name);

        return next(request, cancellationToken);
    }
}
