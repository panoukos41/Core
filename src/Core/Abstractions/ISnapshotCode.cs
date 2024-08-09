namespace Core.Abstractions;

public interface ISnapshotCode
{
    public int GetSnapshotCode();
}

public static class ISnapshotMixins
{
    public static int GetSnapshotCode<T>(this IEnumerable<T> values) where T : ISnapshotCode
    {
        return values.Aggregate(0, static (a, c) => HashCode.Combine(a, c.GetSnapshotCode()));
    }
}
