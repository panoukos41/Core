namespace System.Reactive.Linq;

internal static class IObservableExtensions
{
    public static IObservable<T> WhereNotNull<T>(this IObservable<T?> observable)
    {
        return observable.Where(static x => x is { })!;
    }
}
