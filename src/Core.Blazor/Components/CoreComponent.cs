using Ignis.Components;
using Microsoft.AspNetCore.Components;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;

namespace Core.Blazor.Components;

public abstract class CoreComponent : IgnisComponentBase, IDisposable
{
    private readonly Lazy<CompositeDisposable> disposables = new(static () => []);

    protected CompositeDisposable Disposables => disposables.Value;

    /// <summary>
    /// Add a disposable to the <see cref="Disposables"/> list.
    /// </summary>
    protected void DisposeWith(IDisposable disposable)
    {
        disposables.Value.Add(disposable);
    }

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? Attributes { get; set; }

    public object? TryGetAttribute(string key)
        => Attributes?.TryGetValue(key, out var value) is true ? value : null;

    public T? TryGetAttribute<T>(string key)
        => TryGetAttribute(key) is T  v ? v : default;

    public bool TryGetAttribute(string key, out object attribute)
    {
        Unsafe.SkipInit(out attribute);
        if (Attributes?.TryGetValue(key, out var value) is true && value is { })
        {
            attribute = value;
            return true;
        }
        return false;
    }

    public bool TryGetAttribute<T>(string key, out T attribute)
    {
        Unsafe.SkipInit(out attribute);
        if (TryGetAttribute(key, out var value) && value is T valueAttr)
        {
            attribute = valueAttr;
            return true;
        }
        return false;
    }

    public T GetAttributeOrDefault<T>(string key, T @default)
        => TryGetAttribute<T>(key) ?? @default;

    public virtual void Dispose()
    {
        if (disposables.IsValueCreated)
        {
            disposables.Value.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    //public Task HandleEventAsync(EventCallbackWorkItem item, object? arg)
    //{
    //    Update();
    //    return Task.CompletedTask;
    //}
}
