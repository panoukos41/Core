using Core.Reactive;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;

namespace Core.Abstractions;

public abstract class CoreRxComponent<TRxObject> : CoreComponent where TRxObject : notnull, RxObject
{
    private IDisposable? changedSub;

    private TRxObject viewModel = null!;

    [Parameter, EditorRequired, NotNull]
    public virtual TRxObject ViewModel
    {
        get => viewModel;
        set {
            viewModel = value;
            if (changedSub is { })
            {
                Disposables.Remove(changedSub);
                changedSub.Dispose();
                changedSub = null;
            }
            changedSub = viewModel.WhenPropertyChanged
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Subscribe(_ => TriggerUpdate())
                .DisposeWith(this);
        }
    }
}
