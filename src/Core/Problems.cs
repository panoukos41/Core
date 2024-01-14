using FluentValidation.Results;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Core;

public static class Problems
{
    public static Problem BadRequest { get; } = new()
    {
        Type = nameof(BadRequest),
        Title = nameof(BadRequest),
        Status = 400,
    };

    public static Problem Validation { get; } = new()
    {
        Type = nameof(Validation),
        Title = nameof(Validation),
        Status = 400,
        Detail = "Validation problems have occurred with your model. Check the Metadata:ValidationErrors a list of ValidationFailure objects."
    };

    public static Problem Unauthorized { get; } = new()
    {
        Type = nameof(Unauthorized),
        Title = nameof(Unauthorized),
        Status = 401
    };

    public static Problem Forbid { get; } = new()
    {
        Type = nameof(Forbid),
        Title = nameof(Forbid),
        Status = 403
    };

    public static Problem NotFound { get; } = new()
    {
        Type = nameof(NotFound),
        Title = nameof(NotFound),
        Status = 404
    };

    public static Problem Conflict { get; } = new()
    {
        Type = nameof(Conflict),
        Title = nameof(Conflict),
        Status = 409
    };

    public static Problem Teapot { get; } = new()
    {
        Type = nameof(Teapot),
        Title = nameof(Teapot),
        Status = 418,
        Detail = "I'm a teapot"
    };

    public static Problem Internal { get; } = new()
    {
        Type = nameof(Internal),
        Title = "Internal service error",
        Status = 500
    };
}

public static class ProblemMixins
{
    public static Problem WithMetadata<TMetadata>(this Problem problem, string key, TMetadata metadata)
    {
        if (metadata is null)
            return problem;

        var newProblem = problem with { Metadata = [] };
        newProblem.Metadata[key] = JsonSerializer.SerializeToNode(metadata, Options.Json);
        return newProblem;
    }

    public static Problem WithMetadata(this Problem problem, JsonObject metadata)
    {
        if (metadata is not { Count: > 1 })
            return problem;

        var newProblem = problem with { Metadata = [] };
        newProblem.Metadata
            .CopyFrom(problem?.Metadata)
            .CopyFrom(metadata);
        return newProblem;
    }

    public static Problem WithMetadata(this Problem problem, IDictionary<string, object> metadata)
    {
        if (metadata is not { Count: > 1 })
            return problem;

        var newProblem = problem with { Metadata = [] };
        var jElement = JsonSerializer.SerializeToElement(metadata, Options.Json);
        var jObject = JsonObject.Create(jElement);
        newProblem.Metadata
            .CopyFrom(problem?.Metadata)
            .CopyFrom(jObject);
        return newProblem;
    }

    public static Problem WithValidationErrors(this Problem problem, List<ValidationFailure> failures)
    {
        return WithMetadata(problem, "ValidationFailures", failures);
    }

    public static List<ValidationFailure>? GetValidationFailures(this Problem problem)
    {
        return problem.Metadata?.GetList<ValidationFailure>("ValidationFailures");
    }
}
