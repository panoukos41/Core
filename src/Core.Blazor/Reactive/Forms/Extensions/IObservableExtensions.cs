namespace R3;

internal static class IObservableExtensions
{
    public static Observable<T> WhereNotNull<T>(this Observable<T?> observable)
    {
        return observable.Where(static x => x is { })!;
    }
}
