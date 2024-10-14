namespace System;

public static class EnumerableMixins
{
    public static int GetAggregateHashCode<T>(this IEnumerable<T> values)
    {
        return values.Aggregate(0, static (a, c) => HashCode.Combine(a, c));
    }

    public static IEnumerable<T> Do<T>(this IEnumerable<T> values, Action<T> action)
    {
        foreach (var item in values)
        {
            action(item);
            yield return item;
        }
    }
}
