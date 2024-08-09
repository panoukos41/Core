namespace Core.Abstractions;

public interface ISnapshot
{
    public ReadOnlySpan<byte> GetSnapshot();
}
