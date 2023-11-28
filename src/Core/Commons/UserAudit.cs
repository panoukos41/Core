namespace Core.Commons;

public interface IUserAudit
{
    UserAudit UserAudit { get; }
}

public sealed record UserAudit
{
    public static UserAudit Empty { get; } = new();

    /// <summary>
    /// The name/id of the user that created the object.
    /// </summary>
    public string? CreatedBy { get; init; }

    /// <summary>
    /// The name/id of the last user that updated the object.
    /// </summary>
    public string? UpdatedBy { get; set; }

    public UserAudit(string? createdBy = null, string? updatedBy = null)
    {
    }
}

public static class UserAuditMixins
{
    /// <summary>
    /// Get the <see cref="UserAudit.CreatedBy"/> property of an <see cref="IUserAudit"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the object implementing <see cref="IUserAudit"/>.</typeparam>
    /// <param name="obj">The object implementing <see cref="IUserAudit"/>.</param>
    /// <returns>The created by value.</returns>
    public static string? CreatedBy<T>(this T obj) where T : IUserAudit
    {
        return obj.UserAudit.CreatedBy;
    }

    /// <summary>
    /// Get the <see cref="UserAudit.UpdatedBy"/> property of an <see cref="IUserAudit"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the object implementing <see cref="IUserAudit"/>.</typeparam>
    /// <param name="obj">The object implementing <see cref="IUserAudit"/>.</param>
    /// <returns>The updated by value.</returns>
    public static string? UpdatedBy<T>(this T obj) where T : IUserAudit
    {
        return obj.UserAudit.UpdatedBy;
    }

    /// <summary>
    /// Set the <see cref="UserAudit.UpdatedBy"/> property of an <see cref="IUserAudit"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the object implementing <see cref="IUserAudit"/>.</typeparam>
    /// <param name="obj">The object implementing <see cref="IUserAudit"/>.</param>
    /// <param name="user">The user that updated the object.</param>
    /// <returns>The <paramref name="obj"/>.</returns>
    public static T UpdatedAt<T>(this T obj, string user) where T : IUserAudit
    {
        obj.UserAudit.UpdatedBy = user;
        return obj;
    }
}
