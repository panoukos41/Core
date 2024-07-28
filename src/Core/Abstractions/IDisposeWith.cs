namespace Core.Abstractions;

/// <summary>
/// Indicates that a <see cref="IDisposable"/> object can dispose other objects with it.
/// </summary>
public interface IDisposeWith
{
    /// <summary>
    /// Add a disposable that will be disposed with this instance.
    /// </summary>
    public void DisposeWith(IDisposable disposable);
}
