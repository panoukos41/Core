using Blackwing.Contracts.Requests;
using FluentValidation;

namespace Core.Common.Behaviors;

public sealed class ValidBehavior<TRequest, TResponse> : IRequestPipeline<TRequest, TResponse>
    where TRequest : IRequest, IValid
    where TResponse : IResultUnion
{
    public ValueTask<TResponse> Handle(TRequest request, IRequestPipelineDelegate<TRequest, TResponse> next, CancellationToken cancellationToken = default)
    {
        var validationContext = new ValidationContext<TRequest>(request);
        var validationResult = TRequest.Validator.Validate(validationContext);

        return validationResult.IsValid
            ? next(request, cancellationToken)
            : new((TResponse)TResponse.CreateEr(Problems.Validation.WithValidationFailures(validationResult.Errors)));
    }
}
