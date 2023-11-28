using FluentValidation;
using FluentValidation.Results;

namespace Core.Commons.Behaviors;

public sealed class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
    where TResponse : IResultUnion
{
    private readonly IEnumerable<IValidator<TRequest>>? validators;

    public FluentValidationBehavior(IEnumerable<IValidator<TRequest>>? validators)
    {
        this.validators = validators;
    }

    public async ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
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
                return (TResponse)TResponse.CreateEr(Problems.Validation.WithValidationErrors(validationResults));
            }
        }

        return await next(request, cancellationToken);
    }
}
