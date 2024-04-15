using Core;

namespace System.Reactive.Linq;

public static class IObservableExtensions
{
    public static IObservable<None> ToNone<T>(this IObservable<T> observable)
        => observable.Select(static _ => None.Value);
}
