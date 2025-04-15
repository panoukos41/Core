using Core;
using R3;

namespace System.Reactive.Linq;

public static class ObservableExtensions
{
    public static Observable<Nothing> ToNothing<T>(this Observable<T> observable)
        => observable.Select(static _ => Nothing.Value);
}
