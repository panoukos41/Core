using FluentValidation;
using Core.Abstractions.Requests;

namespace Core.Abstractions;

/// <summary>
/// Declares that a type has a global validator that can validate it.
/// </summary>
/// <remarks>
/// This a generic form like (object to class) of <see cref="IValid{TSelf}"/> interface. <br/>
/// You should only use it for generic things like <see cref="Command{TData, TResult}"/>. <br/>
/// For anything else use <see cref="IValid{TSelf}"/> which inherits <see cref="IValid"/>.
/// </remarks>
public interface IValid
{
    abstract static IValidator Validator { get; }
}

/// <summary>
/// Declares that a type has a global validator that can validate it.
/// </summary>
/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
public interface IValid<TSelf> : IValid where TSelf : IValid<TSelf>
{
    abstract static new IValidator<TSelf> Validator { get; }

    static IValidator IValid.Validator => TSelf.Validator;
}
