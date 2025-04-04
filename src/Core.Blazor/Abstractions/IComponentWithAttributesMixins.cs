﻿using Core.Common;
using System.Runtime.CompilerServices;

namespace Core.Abstractions;

public static class IComponentWithAttributesMixins
{
    public static string? Class(this IComponentWithAttributes component)
    {
        return component.TryGetAttribute<string>("class");
    }

    public static string Class(this IComponentWithAttributes component, string always)
    {
        var componentClass = component.Class();
        return componentClass is null ? always : $"{always} {componentClass}";
    }

    public static string? Class(this IComponentWithAttributes component, ClassLine classLine)
    {
        return Bl.Class([component.Class(), classLine]);
    }

    public static string Class(this IComponentWithAttributes component, params IEnumerable<ClassLine> classes)
    {
        return Bl.Class([component.Class(), .. classes]);
    }

    public static string Class(this IComponentWithAttributes component, string always, ClassLine classLine)
    {
        return component.Class([always, classLine]);
    }

    public static string Class(this IComponentWithAttributes component, string always, params IEnumerable<ClassLine> classes)
    {
        return component.Class([always, .. classes]);
    }

    public static bool Disabled(this IComponentWithAttributes component)
    {
        return component.TryGetAttribute<bool>("disabled");
    }

    public static string? Type(this IComponentWithAttributes component)
    {
        return component.TryGetAttribute<string>("type");
    }

    public static string Type(this IComponentWithAttributes component, string @default)
    {
        return component.Type() ?? @default;
    }

    public static object? TryGetAttribute(this IComponentWithAttributes component, string key)
    {
        return component.Attributes?.TryGetValue(key, out var value) is true ? value : null;
    }

    public static T? TryGetAttribute<T>(this IComponentWithAttributes component, string key)
    {
        return component.TryGetAttribute(key) is T v ? v : default;
    }

    public static bool TryGetAttribute(this IComponentWithAttributes component, string key, out object attribute)
    {
        Unsafe.SkipInit(out attribute);
        if (component.Attributes?.TryGetValue(key, out var value) is true && value is { })
        {
            attribute = value;
            return true;
        }
        return false;
    }

    public static bool TryGetAttribute<T>(this IComponentWithAttributes component, string key, out T attribute)
    {
        Unsafe.SkipInit(out attribute);
        if (component.TryGetAttribute(key, out var value) && value is T valueAttr)
        {
            attribute = valueAttr;
            return true;
        }
        return false;
    }

    public static T GetAttributeOrDefault<T>(this IComponentWithAttributes component, string key, T @default)
    {
        return component.TryGetAttribute<T>(key) ?? @default;
    }
}
