using System.Collections.ObjectModel;

namespace Core.Common.Mixins;

public static class KeyedCollectionMixins
{
    public static void AddOrUpdate<TKey, TItem>(this KeyedCollection<TKey, TItem> items, TItem item) where TKey : notnull
    {
        items.Remove(item);
        items.Add(item);
    }
}
