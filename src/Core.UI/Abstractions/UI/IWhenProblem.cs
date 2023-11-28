namespace Core.Abstractions.UI;

/// <summary>
/// Notifies clients that a problem occurred.
/// </summary>
public interface IWhenProblem
{
    /// <summary>
    /// Observable that ticks when a problem occurres.
    /// </summary>
    IObservable<Problem> WhenProblem { get; }
}
