namespace Core.Common;

public readonly struct ClassLine
{
    public readonly string? Class { get; }

    public ClassLine(string @class, bool condition)
    {
        Class = condition ? @class : null;
    }

    public override string? ToString()
    {
        return Class;
    }

    public static implicit operator ClassLine((string Class, bool Condition) tuple)
    {
        return new(tuple.Class, tuple.Condition);
    }

    public static implicit operator ClassLine(string? Class)
    {
        return string.IsNullOrEmpty(Class) ? new(string.Empty, false) : new(Class, true);
    }
}
