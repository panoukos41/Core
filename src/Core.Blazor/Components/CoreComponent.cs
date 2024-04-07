using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace Core.Blazor.Components;

public abstract class CoreComponent : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? Attributes { get; set; }

    public object? TryGetAttribute(string key)
        => Attributes?.TryGetValue(key, out var value) is true ? value : null;

    public T? TryGetAttribute<T>(string key) where T : class
        => TryGetAttribute(key) as T;

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

    public bool TryGetAttribute<T>(string key, out T attribute) where T : class
    {
        Unsafe.SkipInit(out attribute);
        if (TryGetAttribute(key, out var value) && value is T valueAttr)
        {
            attribute = valueAttr;
            return true;
        }
        return false;
    }
}
