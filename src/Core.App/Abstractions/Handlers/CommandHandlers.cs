using Core.Abstractions.Requests;
using System.Reflection;

namespace Core.Abstractions.Handlers;

public abstract class CommandHandler<TCommand, TResult> :
    ICommandHandler<TCommand, Result<TResult>>
    where TCommand : Command<TResult>
    where TResult : notnull
{
    public abstract ValueTask<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
}
