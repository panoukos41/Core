﻿using Microsoft.AspNetCore.Components;
using System.Reactive.Disposables;

namespace Core.Abstractions;

public abstract class CoreComponent : ComponentBase, IComponentWithAttributes, IDisposeWith, IDisposable
{
    private readonly Lazy<CompositeDisposable> disposables = new(static () => []);

    protected CompositeDisposable Disposables => disposables.Value;

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
