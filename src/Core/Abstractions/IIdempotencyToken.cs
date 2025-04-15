namespace Core.Abstractions;

/// <summary>
/// Represents an interface for objects that include an idempotency token.
/// </summary>
public interface IIdempotencyToken
{
    /// <summary>
    /// Gets the idempotency token, which is a unique identifier used to ensure
    /// that operations are not performed multiple times unintentionally.
    /// </summary>
    string IdempotencyToken { get; }
}
