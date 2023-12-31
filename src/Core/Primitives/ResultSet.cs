using System.Diagnostics.CodeAnalysis;

namespace Core.Primitives;

public sealed record ResultSet<TSet>
{
    public required long Count { get; set; }

    public required IEnumerable<TSet> Items { get; set; }

    public ResultSet()
    {
    }

    [SetsRequiredMembers]
    public ResultSet(long count, IEnumerable<TSet> items)
    {
        Count = count;
        Items = items;
    }

    [SetsRequiredMembers]
    public ResultSet(int count, IEnumerable<TSet> items) : this((long)count, items)
    {
    }
}

public static class ResultSet
{
    public static ResultSet<T> Create<T>(long count, IEnumerable<T> items)
        => new(count, items);

    public static ResultSet<T> Create<T>(int count, IEnumerable<T> items)
        => new(count, items);

    public static ResultSet<T> Create<T>(ICollection<T> items)
        => new(items.Count, items);
}
