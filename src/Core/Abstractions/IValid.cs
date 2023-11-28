using FluentValidation;

namespace Core.Abstractions;

public interface IValid
{
    abstract static IValidator Validator { get; }
}
