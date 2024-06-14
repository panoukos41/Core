using Core.Blazor.Reactive.Forms.Abstract;
using System.Diagnostics.CodeAnalysis;

namespace Core.Blazor.Reactive.Forms.Primitives;

internal readonly struct AbstractControlEntry : IEquatable<AbstractControlEntry>
{
    public static AbstractControlEntry Empty { get; } = new();

    public AbstractControl Control { get; }
    private readonly IDisposable sub;

    public AbstractControlEntry(AbstractControl control, AbstractControl parent, IDisposable sub)
    {
        Control = control;
        Control.Parent = parent;
        this.sub = sub;
    }

    public void Dispose()
    {
        sub.Dispose();
        Control.Parent = null;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is AbstractControlEntry entry && Equals(entry);
    }

    public bool Equals(AbstractControlEntry other)
    {
        return Control == other.Control;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Control);
    }

    public static bool operator ==(AbstractControlEntry l, AbstractControlEntry r)
    {
        return l.Control == r.Control;
    }

    public static bool operator !=(AbstractControlEntry l, AbstractControlEntry r)
    {
        return l.Control != r.Control;
    }
}