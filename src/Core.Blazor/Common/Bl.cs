using System.Text;

namespace Core.Common;

public static class Bl
{
    public static string? Class(string? always)
    {
        return always;
    }

    public static string? Class(ClassLine classLine)
    {
        return classLine.ToString();
    }

    public static string Class(params IEnumerable<ClassLine> classes)
    {
        var sb = new StringBuilder();
        foreach (var @class in classes)
        {
            if (@class.Class is null) continue;

            sb.Append(@class.Class);
            sb.Append(' ');
        }
        if (sb.Length > 0)
        {
            sb.Remove(sb.Length - 1, 1);
        }
        return sb.ToString();
    }

    public static string Class(string always, ClassLine classLine)
    {
        return Class([always, classLine]);
    }

    public static string Class(string always, params IEnumerable<ClassLine> classes)
    {
        return Class([always, .. classes]);
    }
}
