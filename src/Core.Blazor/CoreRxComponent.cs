using Core.Reactive;
using Microsoft.AspNetCore.Components;
using System.Reactive.Linq;

namespace Core.Blazor;

public abstract class CoreRxComponent<TRxObject> : CoreComponent where TRxObject : notnull, RxObject
{
    private IDisposable? changedSub;

    [Parameter, EditorRequired]
    public virtual TRxObject ViewModel { get; set; } = null!;

    protected sealed override void OnUpdate()
    {
        if (ViewModel is null && changedSub is not null)
        {
            Disposables.Remove(changedSub!);
            changedSub?.Dispose();
            changedSub = null;
        }
        else if (ViewModel is { } && changedSub is null)
        {
            changedSub = ViewModel.WhenPropertyChanged
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Subscribe(_ => Update())
                .DisposeWith(Disposables);
        }
    }
}
