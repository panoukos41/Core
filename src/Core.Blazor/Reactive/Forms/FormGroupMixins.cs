using Core.Blazor.Reactive.Forms.Abstract;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Core.Blazor.Reactive.Forms;

public static class FormGroupMixins
{
    public static FormControl<T> GetFormControl<T>(this AbstractControl formGroup, string path)
        where T : IParsable<T>
    {
        return (FormControl<T>)RetrieveControl(formGroup, path);
    }

    public static FormGroup GetFormGroup(this AbstractControl formGroup, string path)
    {
        return (FormGroup)RetrieveControl(formGroup, path);
    }

    public static FormArray GetFormArray(this AbstractControl formGroup, string path)
    {
        return (FormArray)RetrieveControl(formGroup, path);
    }

    public static bool TryGetFormControl<T>(this AbstractControl formGroup, string path, [MaybeNullWhen(false)] out FormControl<T>? control)
        where T : IParsable<T>
    {
        Unsafe.SkipInit(out control);
        try
        {
            control = RetrieveControl(formGroup, path) as FormControl<T>;
            return control is { };
        }
        catch
        {
            return false;
        }
    }

    public static bool TryGetFormGroup(this AbstractControl formGroup, string path, [MaybeNullWhen(false)] out FormGroup group)
    {
        Unsafe.SkipInit(out group);
        try
        {
            group = RetrieveControl(formGroup, path) as FormGroup;
            return group is { };
        }
        catch
        {
            return false;
        }
    }

    public static bool TryGetFormArray(this AbstractControl formGroup, string path, [MaybeNullWhen(false)] out FormArray array)
    {
        Unsafe.SkipInit(out array);
        try
        {
            array = RetrieveControl(formGroup, path) as FormArray;
            return array is { };
        }
        catch
        {
            return false;
        }
    }

    private static readonly SearchValues<char> separators = SearchValues.Create(".[");

    private static AbstractControl RetrieveControl(AbstractControl control, string propertyPath)
    {
        if (string.IsNullOrEmpty(propertyPath))
            return control;

        var span = propertyPath.AsSpan();
        var obj = control;

        while (true)
        {
            if (span.StartsWith("["))
                span = span[1..];

            var nextTokenEnd = span.IndexOfAny(separators);
            if (nextTokenEnd < 0)
                return TryGetControl(propertyPath, span, obj);

            var nextToken = span[..nextTokenEnd];
            span = span[(nextTokenEnd + 1)..];

            obj = TryGetControl(propertyPath, nextToken, obj);
        }
    }

    private static AbstractControl TryGetControl(string propertyPath, ReadOnlySpan<char> token, AbstractControl obj)
    {
        AbstractControl? newObj;
        if (token.EndsWith("]"))
        {
            token = token[..^1];
            if (int.TryParse(token, out var index))
            {
                newObj = TryGetFromArray(obj, index)
                    ?? throw new InvalidOperationException($"For path {propertyPath} could not access token {token} because the previous token was not an IList<AbstractControl> type.");
            }
            else
            {
                newObj = TryGetFromGroup(obj, $"{token}")
                    ?? throw new InvalidOperationException($"For path {propertyPath} could not access token {token} because the previous token was not an IDictionary<string, AbstractControl> type.");
            }
        }
        else
        {
            newObj = TryGetFromGroup(obj, $"{token}")
                ?? throw new InvalidOperationException($"For path {propertyPath} could not access token {token} because the previous token was not an IDictionary<string, AbstractControl> type.");
        }
        if (newObj is null)
            throw new InvalidOperationException($"For path {propertyPath} could not find token {token}");

        return newObj;
    }

    private static AbstractControl? TryGetFromGroup(AbstractControl group, string key)
    {
        return group is IDictionary<string, AbstractControl> dict ? dict[key] : null;
    }

    private static AbstractControl? TryGetFromArray(AbstractControl array, int index)
    {
        return array is IList<AbstractControl> list ? list[index] : null;
    }
}
