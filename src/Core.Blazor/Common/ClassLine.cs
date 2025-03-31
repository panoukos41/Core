namespace Core.Common;

public readonly ref struct ClassLine
{
    public readonly string Class { get; }

    public readonly bool Condition { get; }

    public ClassLine(string @class, bool condition)
    {
        Class = @class;
        Condition = condition;
    }

    public override string? ToString()
    {
        return Condition ? Class : null;
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
