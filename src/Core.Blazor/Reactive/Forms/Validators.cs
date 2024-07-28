using Core.Blazor.Reactive.Forms.Abstract;
using Core.Blazor.Reactive.Forms.Primitives;
using System.Numerics;

namespace Core.Blazor.Reactive.Forms;

public static class Validators
{
    public static ValidatorFn Required { get; } = control =>
    {
        return control switch
        {
            AbstractControl<string> str => string.IsNullOrWhiteSpace(str.Value) ? new ValidationError("required") : null,
            AbstractControl c => c.HasValue ? null : new ValidationError("required"),
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

    public static ValidatorFn Min<TNumber>(TNumber min) where TNumber : INumber<TNumber> => control =>
    {
        if (control is not AbstractControl<TNumber> num || num.Value is null)
            return null;

        var value = num.Value;

        return value < min
            ? new ValidationError("min", new() { ["min"] = $"{min}", ["current"] = $"{value ?? TNumber.Zero}" })
            : null;
    };

    public static ValidatorFn MoreThan<TNumber>(TNumber num) where TNumber : INumber<TNumber> => control =>
    {
        if (control is not AbstractControl<TNumber> str)
            return null;

        var value = str.Value;

        return value is null || value <= num
            ? new ValidationError("min", new() { ["min"] = $"{num}", ["current"] = $"{value ?? TNumber.Zero}" })
            : null;
    };
}
