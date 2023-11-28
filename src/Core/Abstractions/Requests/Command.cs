using FluentValidation;
using Mediator;

namespace Core.Abstractions.Requests;

/// <summary>
/// Represents a base command.
/// </summary>
/// <typeparam name="TResult">The type of the result object.</typeparam>
public abstract record Command<TResult> : ICommand<Result<TResult>>, IRequestId
    where TResult : notnull
{
    public Guid RequestId { get; init; } = Guid.NewGuid();
}

/// <summary>
/// Represents a <see cref="Command{TResult}"/> with <see cref="IValid"/> data.
/// </summary>
/// <typeparam name="TData">The type of the data.</typeparam>
public abstract record Command<TData, TResult> : Command<TResult>, IValid
    where TData : notnull, IValid
    where TResult : notnull
{
    public TData Data { get; }

    protected Command(TData data)
    {
        Data = data;
    }

    public static IValidator Validator { get; } = InlineValidator.For<Command<TData, TResult>>(data =>
    {
        data.RuleFor(x => x.Data).SetValidator((IValidator<TData>)TData.Validator);
    });
}
