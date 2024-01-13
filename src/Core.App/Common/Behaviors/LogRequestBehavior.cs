using Serilog.Events;

namespace Core.Common.Behaviors;

public sealed class LogRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
{
    public ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        var logger = Log.ForContext<TRequest>();

        if (!logger.IsEnabled(LogEventLevel.Information))
            return next(request, cancellationToken);

        var requestType = request.GetType();
        if (request is IBaseQuery)
        {
            logger.Information("Query {Request}", requestType);
        }
        else if (request is IBaseCommand)
        {
            logger.Information("Command {Request}", requestType);
        }
        else
        {
            logger.Information("RQST {Request}", requestType);
        };

        return next(request, cancellationToken);
    }
}
