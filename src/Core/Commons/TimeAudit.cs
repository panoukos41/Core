using System.Diagnostics.CodeAnalysis;

namespace Core.Commons;

public interface ITimeAudit
{
    TimeAudit TimeAudit { get; }
}

public sealed record TimeAudit
{
    public static TimeAudit Empty { get; } = new(createdAt: default);

    /// <summary>
    /// The creation date of the current object.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// The update date of the current object.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    public TimeAudit()
    {
    }

    [SetsRequiredMembers]
    public TimeAudit(TimeProvider timeProvider)
    {
        CreatedAt = timeProvider.GetUtcNow();
    }

    [SetsRequiredMembers]
    public TimeAudit(DateTimeOffset createdAt)
    {
        CreatedAt = createdAt;
    }
}

public static class TimeAuditMixins
{
    /// <summary>
    /// Get the <see cref="TimeAudit.CreatedAt"/> property of an <see cref="ITimeAudit"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the object implementing <see cref="ITimeAudit"/>.</typeparam>
    /// <param name="obj">The object implementing <see cref="ITimeAudit"/>.</param>
    /// <returns>The created time.</returns>
    public static DateTimeOffset CreatedAt<T>(this T obj) where T : ITimeAudit
    {
        return obj.TimeAudit.CreatedAt;
    }

    /// <summary>
    /// Get the <see cref="TimeAudit.UpdatedAt"/> property of an <see cref="ITimeAudit"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the object implementing <see cref="ITimeAudit"/>.</typeparam>
    /// <param name="obj">The object implementing <see cref="ITimeAudit"/>.</param>
    /// <returns>The updated time.</returns>
    public static DateTimeOffset? UpdatedAt<T>(this T obj) where T : ITimeAudit
    {
        return obj.TimeAudit.UpdatedAt;
    }

    /// <summary>
    /// Set the <see cref="TimeAudit.UpdatedAt"/> property of an <see cref="ITimeAudit"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the object implementing <see cref="ITimeAudit"/>.</typeparam>
    /// <param name="obj">The object implementing <see cref="ITimeAudit"/>.</param>
    /// <param name="timeProvider">The provider to get the UTC now time from.</param>
    /// <returns>The <paramref name="obj"/>.</returns>
    public static T UpdatedAt<T>(this T obj, TimeProvider timeProvider) where T : ITimeAudit
    {
        obj.TimeAudit.UpdatedAt = timeProvider.GetUtcNow();
        return obj;
    }

    /// <summary>
    /// Set the <see cref="TimeAudit.UpdatedAt"/> property of an <see cref="ITimeAudit"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the object implementing <see cref="ITimeAudit"/>.</typeparam>
    /// <param name="obj">The object implementing <see cref="ITimeAudit"/>.</param>
    /// <param name="updatedAt">The time to set as update at.</param>
    /// <returns>The <paramref name="obj"/>.</returns>
    public static T UpdatedAt<T>(this T obj, DateTimeOffset updatedAt) where T : ITimeAudit
    {
        obj.TimeAudit.UpdatedAt = updatedAt;
        return obj;
    }
}
