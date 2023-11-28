using System.Diagnostics.CodeAnalysis;

namespace Core.Tools;

public static class InlineEqual
{
    public static InlineEqual<T> For<T>(Func<T, T, bool> equal, Func<T, int>? hashCode = null) => new(equal, hashCode);
}

public sealed class InlineEqual<T> : IEqualityComparer<T>
{
    private readonly Func<T, T, bool> equal;
    private readonly Func<T, int>? hashCode;

    public InlineEqual(Func<T, T, bool> equal, Func<T, int>? hashCode = null)
    {
        this.equal = equal;
        this.hashCode = hashCode;
    }

    public bool Equals(T? x, T? y)
    {
        return x is { } && y is { } && equal(x, y);
    }

    public int GetHashCode([DisallowNull] T obj)
    {
        return hashCode?.Invoke(obj) ?? obj.GetHashCode();
    }
}
