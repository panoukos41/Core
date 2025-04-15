using Microsoft.AspNetCore.Components;
using R3;
using System.Diagnostics.CodeAnalysis;

namespace Core.Abstractions;

public abstract class CoreComponent : ComponentBase, IComponentWithAttributes, IDisposeWith, IDisposable
{
    private readonly Lazy<CompositeDisposable> disposables = new(static () => []);

    protected CompositeDisposable Disposables => disposables.Value;

    /// <summary>
    /// A unique id for the component.
    /// </summary>
    [Parameter]
    [field: AllowNull]
    public string Id
    {
        get => field ??= Uuid.NewUuid();
        set => field = value;
    }

    /// <inheritdoc/>
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? Attributes { get; set; }

    public void Update()
    {
        StateHasChanged();
    }

    /// <inheritdoc/>
    public void DisposeWith(IDisposable disposable)
    {
        disposables.Value.Add(disposable);
    }

    protected virtual void OnDispose()
    {
    }

    public void Dispose()
    {
        if (disposables.IsValueCreated)
        {
            disposables.Value.Dispose();
        }
        OnDispose();
        GC.SuppressFinalize(this);
    }
}
