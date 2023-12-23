namespace Core.Abstractions;

public interface IEmpty<TSelf> where TSelf : IEmpty<TSelf>
{
    abstract static TSelf Empty { get; }
}
