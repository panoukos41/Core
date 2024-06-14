using System.Collections;
using Core.Blazor.Reactive.Forms.Abstract;

namespace Core.Blazor.Reactive.Forms.Primitives;

public class ValidationErrorCollection : IReadOnlyList<IValidationError>
{
    private readonly List<IValidationError> inner;

    public ValidationErrorCollection()
    {
        inner = [];
    }

    public ValidationErrorCollection(IValidationError error)
    {
        inner = [error];
    }

    public ValidationErrorCollection(IEnumerable<IValidationError> errors)
    {
        inner = [.. errors];
    }

    /// <inheritdoc/>
    public int Count
        => inner.Count;

    /// <inheritdoc/>
    public IValidationError this[int index]
        => inner[index];

    public bool Contains(string name)
        => inner.Any(x => x.Key == name);

    public bool Contains(IValidationError error)
        => inner.Contains(error);

    internal void Add(IValidationError error)
        => inner.Add(error);

    internal void AddRange(IEnumerable<IValidationError> errors)
        => inner.AddRange(errors);

    internal void Clear()
        => inner.Clear();

    /// <inheritdoc/>
    public IEnumerator<IValidationError> GetEnumerator()
        => inner.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => inner.GetEnumerator();
}
