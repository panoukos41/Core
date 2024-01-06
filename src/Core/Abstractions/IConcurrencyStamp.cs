namespace Core.Abstractions;

public interface IConcurrencyStamp
{
    string ConcurrencyStamp { get; }
}
