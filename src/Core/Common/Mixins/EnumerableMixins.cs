namespace System;

public static class EnumerableMixins
{
    public static int GetAggregateHashCode<T>(this IEnumerable<T> values)
    {
        return values.Aggregate(0, static (a, c) => HashCode.Combine(a, c));
    }
}
