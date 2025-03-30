using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Core.Components;

public static class Directives
{
    private static Builder? builder = new();
    private static IAttributeDirective[] attributeDirectives = [];
    private static IStructuralDirective[] structuralDirectives = [];

    public static IEnumerable<IAttributeDirective> GetAttributeDirective(IDictionary<string, object?>? attributes)
    {
        return attributeDirectives.Where(d => Select(d, attributes));
    }

    public static IStructuralDirective? GetStructuralDirective(IDictionary<string, object?>? attributes)
    {
        return structuralDirectives.SingleOrDefault(d => Select(d, attributes));
    }

    private static bool Select(IElementDirective directive, IDictionary<string, object?>? attributes)
    {
        // todo: Implement selector
        return attributes is { Count: > 0 } && attributes.ContainsKey(directive.Selector);
    }

    static Directives()
    {
        Register(new HighlightDirective());
        Register(new IfDirective());
        Build();
    }

    public static void Register(IAttributeDirective directive)
    {
        Check();
        builder.AttributeDirectives.Add(directive);
    }

    public static void Register(IStructuralDirective directive)
    {
        Check();
        builder.StructuralDirectives.Add(directive);
    }

    // todo: make tests so that structural directives don't choose the same element only one can stay

    public static void Build()
    {
        Check();
        attributeDirectives = [.. builder.AttributeDirectives];
        structuralDirectives = [.. builder.StructuralDirectives];
        builder = null;
    }

    [MemberNotNull(nameof(builder))]
    private static void Check()
    {
        if (builder is null)
        {
            throw new InvalidOperationException("Directives collection has already been built.");
        }
    }

    private sealed class Builder
    {
        public List<IAttributeDirective> AttributeDirectives { get; } = [];
        public List<IStructuralDirective> StructuralDirectives { get; } = [];
    }
}

public sealed class Element : ComponentBase
{
    private readonly IServiceProvider? serviceProvider;

    [Parameter, EditorRequired]
    public string Tag { get; set; } = string.Empty;

    /// <inheritdoc/>
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? Attributes { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override bool ShouldRender()
    {
        return !string.IsNullOrWhiteSpace(Tag);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        Print(Attributes);

        if (Attributes is not null)
        {
            foreach (var attributeDirective in Directives.GetAttributeDirective(Attributes))
            {
                Console.WriteLine($"FOUND DIRECTIVE: {attributeDirective.GetType().Name}");
                attributeDirective.Apply(Attributes);
            }
        }

        Print(Attributes);

        // get all attribute directives here.

        // execute structural directive here which should do all the open, add attributes etc.
        // maybe pass some methods or create base class to make it easier.

        // if the directive does nothing then execute the below code.

        if (Directives.GetStructuralDirective(Attributes) is { } structuralDirective &&
            structuralDirective.Apply(Tag, builder, Attributes!, ChildContent))
        {
            return;
        }

        builder.OpenElement(0, Tag);
        builder.AddMultipleAttributes(1, Attributes!);
        builder.AddContent(2, ChildContent);
        builder.CloseElement();
    }

    private static void Print(IDictionary<string, object?>? attributes, [CallerArgumentExpression(nameof(attributes))] string? name = null)
    {
        if (attributes is null) return;

        Console.WriteLine($"--- {name}");
        if (attributes is not null)
        {
            foreach (var (k, v) in attributes)
            {
                Console.WriteLine($"""- "{k}"="{v}" ({v?.GetType().Name ?? "null"}) """);
            }
        }
        Console.WriteLine("---");
        Console.WriteLine();
    }
}

public sealed class HighlightDirective : IAttributeDirective
{
    //public string Selector { get; } = "[appHighlight]";
    public string Selector { get; } = "appHighlight";

    public void Apply(IDictionary<string, object?> attributes)
    {
        const string red = "background: red;";

        var style = attributes.TryGetValue("style", out var styleValue) ? styleValue as string : "";
        if (string.IsNullOrEmpty(style))
        {
            attributes["style"] = red;
        }
        else if (style.EndsWith(';'))
        {
            attributes["style"] = $"{style} {red}";
        }
        else
        {
            attributes["style"] = $"{style}; {red}";
        }
    }
}

public sealed class IfDirective : IStructuralDirective
{
    public string Selector { get; } = "*if";

    public bool Apply(string tag, RenderTreeBuilder builder, IDictionary<string, object?> attributes, RenderFragment? childContent)
    {
        if (attributes[Selector] is true)
        {
            builder.OpenElement(0, tag);
            builder.AddMultipleAttributes(1, attributes.Where(x => char.IsLetter(x.Key[0]))!);
            builder.AddContent(2, childContent);
            builder.CloseElement();
        }

        return true;
    }
}

public interface IElementDirective
{
    string Selector { get; }
}

public interface IAttributeDirective : IElementDirective
{
    void Apply(IDictionary<string, object?> attributes);
}

public interface IStructuralDirective : IElementDirective
{
    bool Apply(string tag, RenderTreeBuilder builder, IDictionary<string, object?> attributes, RenderFragment? childContent);
}
