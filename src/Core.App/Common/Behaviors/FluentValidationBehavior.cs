using Blackwing.Contracts.Requests;
using FluentValidation;
using FluentValidation.Results;

namespace Core.Common.Behaviors;

public sealed class FluentValidationBehavior<TRequest, TResponse> : IRequestPipeline<TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : IResultUnion
{
    private readonly IEnumerable<IValidator<TRequest>>? validators;

    public FluentValidationBehavior(IEnumerable<IValidator<TRequest>>? validators)
    {
        this.validators = validators;
    }

    public async ValueTask<TResponse> Handle(TRequest request, IRequestPipelineDelegate<TRequest, TResponse> next, CancellationToken cancellationToken = default)
    {
        if (validators is not null)
        {
            IValidationContext validationContext = new ValidationContext<TRequest>(request);
            var validationResults = new List<ValidationFailure>();

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(validationContext, cancellationToken);
                validationResults.AddRange(result.Errors);
            }

            if (validationResults.Count > 0)
            {
                return (TResponse)TResponse.CreateEr(Problems.Validation.WithValidationFailures(validationResults));
            }
        }

        return await next(request, cancellationToken);
    }
}
