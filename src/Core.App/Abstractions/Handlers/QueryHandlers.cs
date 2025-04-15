using Blackwing.Contracts.Requests;
using Core.Abstractions.Requests;

namespace Core.Abstractions.Handlers;

public abstract class QueryHandler<TQuery, TResult> :
    IRequestHandler<TQuery, Result<TResult>>
    where TQuery : Query<TResult>
    where TResult : notnull
{
    public abstract ValueTask<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken);
}
