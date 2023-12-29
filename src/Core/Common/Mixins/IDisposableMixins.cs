namespace System;

public static class IDisposableMixins
{
    public static void DisposeWith(this IDisposable disposable, ICollection<IDisposable> disposables)
    {
        disposables.Add(disposable);
    }
}
