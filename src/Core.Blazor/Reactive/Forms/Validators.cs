using Core.Blazor.Reactive.Forms.Abstract;
using Core.Blazor.Reactive.Forms.Primitives;

namespace Core.Blazor.Reactive.Forms;

public static class Validators
{
    public static ValidatorFn Required { get; } = control =>
    {
        return control switch
        {
            AbstractControl<string> str => string.IsNullOrWhiteSpace(str.Value) ? new ValidationError("required") : null,
            { RawValue: null } => new ValidationError("required"),
            _ => null
        };
    };

    public static ValidatorFn MinLength(int minLength) => control =>
    {
        if (control is not AbstractControl<string> str || str.Value is null or "")
            return null;

        var length = str.Value.Length;

        return length < minLength
            ? new ValidationError("minLength", new() { ["min"] = $"{minLength}", ["current"] = $"{length}" })
            : null;
    };
}
