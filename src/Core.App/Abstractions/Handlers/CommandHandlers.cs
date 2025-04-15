using Blackwing.Contracts.Requests;
using Core.Abstractions.Requests;

namespace Core.Abstractions.Handlers;

public abstract class CommandHandler<TCommand, TResult> :
    IRequestHandler<TCommand, Result<TResult>>
    where TCommand : Command<TResult>
    where TResult : notnull
{
    public abstract ValueTask<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
}
