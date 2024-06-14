using Core.Blazor.Reactive.Forms.Abstract;
using System.Runtime.InteropServices;

namespace Core.Blazor.Reactive.Forms.Primitives;

/// <summary>
/// A function that receives a control and synchronously returns a
/// map of validation errors if present, otherwise null.
/// </summary>
public delegate IValidationError? ValidatorFn(AbstractControl control);

/// <summary>
/// A function that receives a control and asynchronously or synchronously returns a
/// map of validation errors if present, otherwise null.
/// </summary>
public delegate ValueTask<IValidationError?> ValidatorFnAsync(AbstractControl control, CancellationToken token = default);

[StructLayout(LayoutKind.Explicit)]
public readonly record struct ValidatorEntry : IEquatable<ValidatorEntry>, IEquatable<ValidatorFn>, IEquatable<ValidatorFnAsync>
{
    [FieldOffset(0)]
    private readonly ValidatorFn? sync;
    [FieldOffset(0)]
    private readonly ValidatorFnAsync? async;
    [FieldOffset(0)]
    private readonly object? stored;

    public ValidatorEntry(ValidatorFn validator)
    {
        sync = validator;
    }

    public ValidatorEntry(ValidatorFnAsync validator)
    {
        async = validator;
    }

    public bool IsSync => stored is ValidatorFn;

    public bool IsAsync => stored is ValidatorFnAsync;

    public IValidationError? Validate(AbstractControl control) => stored switch
    {
        ValidatorFn sync => sync(control),
        _ => default
    };

    public ValueTask<IValidationError?> ValidateAsync(AbstractControl control, CancellationToken cancellationToken = default) => stored switch
    {
        ValidatorFn sync => new(sync(control)),
        ValidatorFnAsync async => async(control, cancellationToken),
        _ => new(default(IValidationError))
    };

    public override string? ToString()
    {
        return stored?.ToString();
    }

    public bool Equals(ValidatorEntry other)
    {
        if (IsSync && other.IsSync)
            return sync == other.sync;
        if (IsAsync && other.IsAsync)
            return async == other.async;
        return false;
    }

    public bool Equals(ValidatorFn? other)
    {
        return IsSync && sync == other;
    }

    public bool Equals(ValidatorFnAsync? other)
    {
        return IsAsync && async == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(stored);
    }
}
