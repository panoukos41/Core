using System.Diagnostics.CodeAnalysis;

namespace Core;

public sealed record ResultSet<TSet>
{
    public required int Count { get; set; }

    public required IEnumerable<TSet> Items { get; set; }

    public ResultSet()
    {
    }

    [SetsRequiredMembers]
    public ResultSet(int count, IEnumerable<TSet> items)
    {
        Count = count;
        Items = items;
    }


    [SetsRequiredMembers]
    public ResultSet(long count, IEnumerable<TSet> items) : this((int)count, items)
    {
    }
}
