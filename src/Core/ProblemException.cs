namespace Core;

public sealed class ProblemException : Exception
{
    public Problem Problem { get; }

    public ProblemException(Problem problem) : base($"{problem.Title}: {problem.Detail}", problem.Exception)
    {
        Problem = problem;
    }
}
