using Serilog.Events;

namespace Core.Commons.Behaviors;

public sealed class LogRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
{
    public ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        var logger = Log.ForContext<TRequest>();

        if (!logger.IsEnabled(LogEventLevel.Information))
            return next(request, cancellationToken);

        if (request is IBaseQuery)
        {
            logger.Information("Query {Name}", request.GetType().Name);
        }
        else if (request is IBaseCommand)
        {
            logger.Information("Command {Name}", request.GetType().Name);
        }
        else
        {
            logger.Information("RQST {Name}", request.GetType().Name);
        };

        return next(request, cancellationToken);
    }
}
