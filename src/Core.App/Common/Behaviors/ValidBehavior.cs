﻿using FluentValidation;

namespace Core.Common.Behaviors;

public sealed class ValidBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage, IValid
    where TResponse : IResultUnion
{
    public ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        var validationContext = new ValidationContext<TRequest>(message);
        var validationResult = TRequest.Validator.Validate(validationContext);

        return validationResult.IsValid
            ? next(message, cancellationToken)
            : new((TResponse)TResponse.CreateEr(Problems.Validation.WithValidationFailures(validationResult.Errors)));
    }
}
