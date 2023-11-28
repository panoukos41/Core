using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Core;

/// <summary>
/// A format similar to ProblemDetails with some extras.
/// </summary>
/// <remarks>Two problems are equal if they have the same type.</remarks>
[DebuggerDisplay("{Type} ({Status})\n{Title}:{Detail}")]
public sealed record Problem
{
    /// <summary>
    /// The type of this problem eg: 'NotFound'.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// A short, summary of the problem type.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The HTTP status code.
    /// </summary>
    public int? Status { get; init; }

    /// <summary>
    /// An explanation specific to this occurrence of the problem.
    /// </summary>
    public string? Detail { get; init; }

    /// <summary>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? Metadata { get; init; }

    /// <summary>
    /// Gets the <see cref="System.Exception"/> instance that caused the current problem.
    /// </summary>
    /// <returns>
    /// The same value as passed into the constructor or initializer
    /// otherwise <see langword="null"/> if no value was provided.
    /// This property is a read-only.
    /// </returns>
    [JsonIgnore]
    public Exception? Exception { get; init; }

    /// <summary>
    /// Test whether the <see cref="Exception"/> or <see cref="Type"/> property is equal to <typeparamref name="TException"/>.
    /// </summary>
    /// <typeparam name="TException">The type of exception to check for.</typeparam>
    /// <remarks>Type will be tested against the name with no namespace.</remarks>
    public bool Is<TException>() => Exception is TException || typeof(TException).Name.Equals(Type, StringComparison.Ordinal);

    public Problem()
    {
    }

    [SetsRequiredMembers]
    public Problem(string type, string title, int? status = null, string? detail = null, JsonObject? metadata = null, Exception? exception = null)
    {
        Type = type;
        Title = title;
        Status = status;
        Detail = detail;
        Metadata = metadata;
    }

    [SetsRequiredMembers]
    public Problem(Exception ex)
    {
        var type = ex.GetType();

        Type = type.Name;
        Title = type.FullName ?? string.Empty;
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
        return Type.GetHashCode();
    }

    public static implicit operator Problem(Exception ex) => new(ex);
}
