using Blackwing.Contracts.Requests;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Core.Abstractions.Requests;

/// <summary>
/// Represents a base command.
/// </summary>
/// <typeparam name="TResult">The type of the result object.</typeparam>
public abstract record Command<TResult> : IRequest<Result<TResult>>
    where TResult : notnull
{
    [JsonPropertyOrder(0)]
    public Guid RequestId { get; init; } = Guid.NewGuid();
}

/// <summary>
/// Represents a <see cref="Command{TResult}"/> with <see cref="IValid{TSelf}"/> data.
/// </summary>
/// <typeparam name="TData">The type of the data.</typeparam>
public abstract record Command<TData, TResult> : Command<TResult>, IValid
    where TData : notnull, IValid<TData>
    where TResult : notnull
{
    [JsonPropertyOrder(100)]
    public required TData Data { get; init; }

    public Command()
    {
    }

    [SetsRequiredMembers]
    protected Command(TData data)
    {
        Data = data;
    }

    public static IValidator Validator { get; } = InlineValidator.For<Command<TData, TResult>>(data =>
    {
        data.RuleFor(x => x.Data).Valid();
    });
}

/// <summary>
/// Represents a delete command. It accepts a <see cref="Uuid"/> of the resource to delete.
/// </summary>
public abstract record DeleteCommand : Command<None>, IValid<DeleteCommand>
{
    public required Uuid Id { get; init; }

    protected DeleteCommand()
    {
    }

    [SetsRequiredMembers]
    protected DeleteCommand(Uuid id)
    {
        Id = id;
    }

    [SetsRequiredMembers]
    protected DeleteCommand(IEntity model)
    {
        Id = model.Id;
    }

    public static IValidator<DeleteCommand> Validator { get; } = InlineValidator.For<DeleteCommand>(data =>
    {
        data.RuleFor(x => x.Id)
            .NotEmpty();
    });
}
