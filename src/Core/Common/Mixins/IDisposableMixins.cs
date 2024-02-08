namespace System;

public static class IDisposableMixins
{
    public static TDisposable DisposeWith<TDisposable>(this TDisposable disposable, ICollection<IDisposable> disposables)
        where TDisposable : IDisposable
    {
        disposables.Add(disposable);
        return disposable;
    }
}
