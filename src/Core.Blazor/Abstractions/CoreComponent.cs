using Ignis.Components;
using Microsoft.AspNetCore.Components;
using System.Reactive.Disposables;

namespace Core.Abstractions;

public abstract class CoreComponent : IgnisComponentBase, IComponentWithAttributes, IDisposeWith, IDisposable
{
    private readonly Lazy<CompositeDisposable> disposables = new(static () => []);

    protected CompositeDisposable Disposables => disposables.Value;

    /// <inheritdoc/>
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? Attributes { get; set; }

    public void TriggerUpdate()
    {
        Update();
    }

    /// <inheritdoc/>
    public void DisposeWith(IDisposable disposable)
    {
        disposables.Value.Add(disposable);
    }

    public virtual void Dispose()
    {
        if (disposables.IsValueCreated)
        {
            disposables.Value.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
