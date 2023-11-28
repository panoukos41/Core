namespace System;

public static class RangeMixins
{
    public static IEnumerable<TResult> Select<TResult>(this Range range, Func<int, TResult> selector)
    {
        return Enumerable
            .Range(range.Start.Value, range.End.Value)
            .Select(selector);
    }
}
