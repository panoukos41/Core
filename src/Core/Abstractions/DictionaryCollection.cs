using System.Collections;

namespace Core.Abstractions;

public abstract class DictionaryCollection<TKey, TItem> : ICollection<TItem>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TItem> dictionary = [];

    public bool IsReadOnly { get; }

    public int Count => dictionary.Count;

    public TItem this[TKey key]
    {
        get => dictionary[key];
    }

    public abstract TKey GetKeyForItem(TItem item);

    public bool Contains(TItem item)
    {
        var key = GetKeyForItem(item);
        return dictionary.ContainsKey(key);
    }

    public void Add(TItem item)
    {
        var key = GetKeyForItem(item);
        dictionary.Add(key, item);
    }

    public void AddOrUpdate(TItem item)
    {
        var key = GetKeyForItem(item);
        dictionary[key] = item;
    }

    public bool Remove(TItem item)
    {
        var key = GetKeyForItem(item);
        return dictionary.Remove(key);
    }

    public void Clear()
    {
        dictionary.Clear();
    }

    public void CopyTo(TItem[] array, int arrayIndex)
    {
        dictionary.Values.CopyTo(array, arrayIndex);
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        return dictionary.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

