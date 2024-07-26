using FluentValidation.Results;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Core;

/// <summary>
/// A format similar to ProblemDetails with some extras.
/// </summary>
/// <remarks>
/// Two problems are equal if they have the same <see cref="Type"/>.
/// </remarks>
[DebuggerDisplay("{Type} ({Status})\n{Title}:{Detail}")]
public sealed record Problem
{
    /// <summary>
    /// The type of the problem. eg: 'NotFound'.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = nameof(Problem);

    /// <summary>
    /// A short, summary of the problem.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; init; } = nameof(Problem);

    /// <summary>
    /// A status code that represents this problem. eg: an HTTP status code.
    /// </summary>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Status { get; init; }

    /// <summary>
    /// A detailed explanation of the problem.
    /// </summary>
    [JsonPropertyName("detail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Detail { get; init; }

    /// <summary>
    /// Additional metadata about the problem.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? Metadata { get; init; }

    /// <summary>
    /// The exception that caused this problem if any.
    /// </summary>
    [JsonIgnore]
    public Exception? Exception { get; init; }

    /// <summary>
    /// A list containing validation failures.
    /// </summary>
    public List<ValidationFailure>? ValidationFailures { get; init; }

    /// <summary>
    /// Test whether the <see cref="Exception"/> or <see cref="Type"/> property is equal to <typeparamref name="TException"/>.
    /// </summary>
    /// <typeparam name="TException">The type of exception to check for.</typeparam>
    public bool Is<TException>()
    {
        if (Exception is TException) return true;

        var type = typeof(TException);
        return type.FullName is { } fullName && fullName == Type
            || type.Name == Type;
    }

    /// <summary>
    /// Initializes a new <see cref="Problem"/> instance.
    /// </summary>
    public Problem()
    {
    }

    /// <summary>
    /// Initializes a new <see cref="Problem"/> instance.
    /// </summary>
    /// <param name="type">The type of the problem. eg: 'NotFound'.</param>
    /// <param name="title">A short, summary of the problem.</param>
    /// <param name="status">A status code that represents this problem. eg: an HTTP status code.</param>
    /// <param name="detail">A detailed explanation of the problem.</param>
    /// <param name="metadata">Additional metadata about the problem.</param>
    /// <param name="exception">The exception that caused this problem if any.</param>
    public Problem(string? type = null, string? title = null, int? status = null, string? detail = null, JsonObject? metadata = null, Exception? exception = null)
    {
        Type = type ?? nameof(Problem);
        Title = title ?? nameof(Problem);
        Status = status;
        Detail = detail;
        Metadata = metadata;
    }

    /// <summary>
    /// Initializes a new <see cref="Problem"/> instance from an exception.
    /// </summary>
    /// <remarks>
    /// Type = ex.GetType().(FullName ?? Name) <br />
    /// Title = ex.GetType().(Name ?? Problem) <br />
    /// Detail = ex.message <br />
    /// Exception = ex
    /// </remarks>
    public Problem(Exception ex)
    {
        var type = ex.GetType();

        Type = type.FullName ?? type.Name;
        Title = type.Name ?? nameof(Problem);
        Detail = ex.Message;
        Exception = ex;
    }

    /// <inheritdoc/>
    public bool Equals(Problem? other)
    {
        return Type == other?.Type;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Type);
    }

    /// <summary>
    /// Implicit operator to transform an <see cref="System.Exception"/> to a <see cref="Problem"/> type.
    /// </summary>
    public static implicit operator Problem(Exception ex) => new(ex);

    /// <summary>
    /// Creates a new instance of <see cref="Problem"/> with the specified detail.
    /// </summary>
    /// <param name="detail">A detailed explanation of the problem.</param>
    public static Problem Create(string detail) => new(detail: detail);

    /// <summary>
    /// Creates a new instance of <see cref="Problem"/> with the specified type and detail.
    /// </summary>
    /// <param name="type">The type of the problem. eg: 'NotFound'.</param>
    /// <param name="status">A status code that represents this problem. eg: an HTTP status code.</param>
    /// <param name="detail">A detailed explanation of the problem.</param>
    public static Problem Create(string? type = null, string? detail = null, int? status = null) => new(type, detail: detail, status: status);

    /// <summary>
    /// Creates a new instance of <see cref="Problem"/> with the specified type, detail, and HTTP status.
    /// </summary>
    /// <param name="type">The type of the problem. eg: 'NotFound'.</param>
    /// <param name="status">A status code that represents this problem. eg: an HTTP status code.</param>
    /// <param name="detail">A detailed explanation of the problem.</param>
    public static Problem Create(string type, string detail, int status) => new(type, null, status, detail);
}
