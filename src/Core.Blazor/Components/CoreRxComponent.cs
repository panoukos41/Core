using Core.Reactive;
using Microsoft.AspNetCore.Components;
using System.Reactive.Linq;

namespace Core.Blazor.Components;

public abstract class CoreRxComponent<TRxObject> : CoreComponent where TRxObject : RxObject
{
    private IDisposable? changedSub;

    [Parameter, EditorRequired]
    public virtual TRxObject? ViewModel { get; set; }

    //protected override bool ShouldRender => ViewModel is not null;

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

    //protected override void OnInitialized()
    //{
    //    changedSub = ViewModel?.WhenPropertyChanged.Subscribe(_ => StateHasChanged());
    //}

    //protected override void OnParametersSet()
    //{
    //    if (ViewModel is null)
    //    {
    //        changedSub?.Dispose();
    //        changedSub = null;
    //    }
    //    else if (ViewModel is { } && changedSub is null)
    //    {
    //        changedSub = ViewModel.WhenPropertyChanged.Subscribe(_ => StateHasChanged());
    //        DisposeWith(changedSub);
    //    }
    //}

    //protected override void BuildRenderTree(RenderTreeBuilder builder)
    //{
    //    if (!ShouldRender) return;

    //    base.BuildRenderTree(builder);
    //}
}
