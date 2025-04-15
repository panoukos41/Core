using Core.Reactive;
using Microsoft.AspNetCore.Components;
using R3;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;

namespace Core.Abstractions;

public abstract class CoreRxComponent<TRxObject> : CoreComponent where TRxObject : notnull, ObservableObject
{
    private IDisposable? changedSub;

    private TRxObject viewModel = null!;

    [Parameter, EditorRequired, NotNull]
    public virtual TRxObject ViewModel
    {
        get => viewModel;
        set {
            viewModel = value;
            if (viewModel.Disposed) return;
            if (changedSub is { })
            {
                Disposables.Remove(changedSub);
                changedSub.Dispose();
                changedSub = null;
            }
            changedSub = viewModel.WhenPropertyChanged
                .Debounce(TimeSpan.FromMilliseconds(50))
                .Subscribe(_ => Update())
                .DisposeWith(this);
        }
    }
}
