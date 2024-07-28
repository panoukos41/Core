﻿using Core.Blazor.Reactive.Forms;
using Ignis.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Core.Components.Forms;

public sealed class RxFormGroup : DynamicComponentBase<RxFormGroup>
{
    private FormGroup? group;

    public RxFormGroup() : this(typeof(Fragment))
    {
        SetAttributes([]);
    }

    public RxFormGroup(string asElement) : base(asElement)
    {
    }

    public RxFormGroup(Type asComponent) : base(asComponent)
    {
    }

    [Parameter]
    public FormGroup? FormGroup { get; set; }

    [CascadingParameter]
    public FormGroup? FormGroupParent { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void OnInitialized()
    {
        group = FormGroup ?? FormGroupParent ?? new();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<FormGroup>>(0);
        builder.AddAttribute(1, nameof(CascadingValue<FormGroup>.IsFixed), value: true);
        builder.AddAttribute(2, nameof(CascadingValue<FormGroup>.Value), group);
        builder.AddAttribute(3, nameof(CascadingValue<FormGroup>.ChildContent), (RenderFragment)(builder =>
        {
            builder.OpenAs(4, this);
            builder.AddMultipleAttributes(5, Attributes!);
            builder.AddChildContentFor(6, this, ChildContent);
            builder.CloseAs(this);
        }));
        builder.CloseComponent();
    }
}
