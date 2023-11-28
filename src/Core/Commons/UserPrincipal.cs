using System.Collections;
using System.Security.Claims;
using System.Security.Principal;

namespace Core.Commons;

public sealed class UserPrincipal : ClaimsPrincipal, IEnumerable<Claim>
{
    public Claim? this[string key]
    {
        get => FindFirst(key);
    }

    /// <summary>
    /// Gets the authentication type that can be used to determine how the primary <see cref="ClaimsIdentity"/> authenticated to an authority.
    /// </summary>
    public string? AuthenticationType => Identity?.AuthenticationType;

    /// <summary>
    /// Gets a value that indicates if the primary <see cref="ClaimsIdentity"/> has been authenticated.
    /// </summary>
    public bool IsAuthenticated => Identity?.IsAuthenticated ?? false;

    public UserPrincipal() : base()
    {
    }

    public UserPrincipal(IPrincipal principal) : base(principal)
    {
    }

    /// <inheritdoc/>
    public IEnumerator<Claim> GetEnumerator()
    {
        return Claims.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Claims.GetEnumerator();
    }
}
