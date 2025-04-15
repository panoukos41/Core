using Blackwing.Contracts.Requests;
using FluentValidation;

namespace Core.Abstractions.Requests;

/// <summary>
/// Represents a query request.
/// </summary>
/// <typeparam name="TResult">The type of the result object.</typeparam>
public abstract record Query<TResult> : IRequest<Result<TResult>> where TResult : notnull
{
}

/// <summary>
/// Represents a list query request.
/// </summary>
/// <typeparam name="TResult">The type of the result object.</typeparam>
public abstract record ListQuery<TResult> : Query<TResult> where TResult : notnull
{
    public int? Page { get; set; }

    public int? Size { get; set; }
}

/// <summary>
/// Represents a <see cref="Query{TResult}"/> that will search by Id.
/// </summary>
/// <typeparam name="TResult">The type of the result object.</typeparam>
public abstract record FindQuery<TResult> : Query<TResult>, IValid<FindQuery<TResult>> where TResult : notnull
{
    public Uuid Id { get; }

    protected FindQuery(Uuid id)
    {
        Id = id;
    }

    public static IValidator<FindQuery<TResult>> Validator { get; } = InlineValidator.For<FindQuery<TResult>>(data =>
    {
        data.RuleFor(x => x.Id)
            .NotEmpty();
    });
}
