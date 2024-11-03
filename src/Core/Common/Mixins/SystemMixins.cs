using System.Diagnostics;

namespace System;

public static class SystemMixins
{
    /// <summary>
    /// Does a hard cast of the object to T.  *Will* throw InvalidCastException
    /// </summary>
    /// <typeparam name="T">The type to cast to.</typeparam>
    /// <param name="target">The object to be casted.</param>
    /// <returns>The <paramref name="target"/> casted to <typeparamref name="T"/>.</returns>
    [StackTraceHidden]
    public static T To<T>(this object target)
    {
        return (T)target;
    }

    /// <summary>
    /// Does a safe cast of the object to T.
    /// </summary>
    /// <typeparam name="T">The type to cast to.</typeparam>
    /// <param name="target">The object to be casted.</param>
    /// <returns>The <paramref name="target"/> casted to <typeparamref name="T"/>.</returns>
    [StackTraceHidden]
    public static T? As<T>(this object target) where T : class
    {
        return target as T;
    }
}
