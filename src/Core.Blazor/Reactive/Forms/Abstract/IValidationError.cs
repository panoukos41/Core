namespace Core.Blazor.Reactive.Forms.Abstract;

public interface IValidationError
{
    /// <summary>
    /// The key of the validation error. This should be tied to the validator
    /// eg: RequiredValidator produces a Validation error with key Required.
    /// </summary>
    public string Key { get; }
}
