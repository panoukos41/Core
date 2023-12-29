namespace System.Security.Claims;

public static class ClaimsPrincipalMixins
{
    public static T? FindFirst<T>(this ClaimsPrincipal user, string type)
        where T : IParsable<T>
    {
        var value = user.FindFirst(type)?.Value;
        return T.TryParse(value, null, out var result) ? result : default;
    }
}
