using Dunet;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core;

/// <summary>
/// Marker interface for the <see cref="Result{T}"/> Union providing static methods for creation.
/// </summary>
public interface IResultUnion
{
    static abstract IResultUnion CreateOk<TOk>(TOk value) where TOk : notnull;

    static abstract IResultUnion CreateEr(Problem problem);

    bool IsOk();

    bool IsEr();

    bool IsEr([NotNullWhen(true)] out Problem? problem);
}

[Union]
[JsonConverter(typeof(ResultJsonConverter))]
public abstract partial record Result<T> : IResultUnion where T : notnull
{
    public static implicit operator Result<T>(Exception ex) => new Er(ex);

    public static IResultUnion CreateOk<TOk>(TOk value) where TOk : notnull => new Result<TOk>.Ok(value);

    public static IResultUnion CreateEr(Problem problem) => new Er(problem);

    [JsonConverter(typeof(ResultJsonConverter))]
    public partial record Ok(T Value);

    [JsonConverter(typeof(ResultJsonConverter))]
    public partial record Er(Problem Problem);

    public bool IsOk() => this is Ok;

    public bool IsOk([NotNullWhen(true)] out T? value)
    {
        value = default;
        if (this is Ok ok)
        {
            value = ok.Value;
            return true;
        }
        return false;
    }

    public bool IsOk([NotNullWhen(true)] out T? value, [NotNullWhen(false)] out Problem? problem)
    {
        Unsafe.SkipInit(out problem);
        if (IsOk(out value)) return true;

        problem = ((Er)this).Problem;
        return false;
    }

    public bool IsEr() => this is Er;

    public bool IsEr([NotNullWhen(true)] out Problem? problem)
    {
        Unsafe.SkipInit(out problem);
        if (this is Er er)
        {
            problem = er.Problem;
            return true;
        }
        return false;
    }

    public bool IsEr([NotNullWhen(true)] out Problem? problem, [NotNullWhen(false)] out T? value)
    {
        Unsafe.SkipInit(out value);
        if (IsEr(out problem)) return true;

        value = ((Ok)this).Value;
        return false;
    }

    public virtual bool Equals(Result<T>? other)
    {
        if (this is Ok ok &&
            other is Ok okOther)
        {
            return EqualityComparer<T>.Default.Equals(ok.Value, okOther.Value);
        }

        var er = this as Er;
        var erOther = other as Er;

        return er is { }
            && erOther is { }
            && er.Problem == erOther.Problem;
    }

    public virtual bool Equals(T? other)
    {
        return other is { } && IsOk(out var value) && EqualityComparer<T>.Default.Equals(value, other);
    }

    public virtual bool Equals(Problem? other)
    {
        return other is { } && IsEr(out var problem) && problem == other;
    }

    public override int GetHashCode() => Match(
        static ok => HashCode.Combine(ok.Value.GetHashCode()),
        static er => HashCode.Combine(er.Problem.GetHashCode())
    );
}

public static class Result
{
    public static Result<TResult> Run<TResult>(this Func<TResult> func) where TResult : notnull
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static Result<TResult> Run<TState, TResult>(TState state, Func<TState, TResult> func) where TResult : notnull
    {
        try
        {
            return func(state);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static async Task<Result<TResult>> RunAsync<TResult>(Func<Task<TResult>> func) where TResult : notnull
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static async Task<Result<TResult>> RunAsync<TState, TResult>(TState state, Func<TState, Task<TResult>> func) where TResult : notnull
    {
        try
        {
            return await func(state);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static Result<TResult> Chain<TResult>(this Result<TResult> result, Func<TResult, Result<TResult>> chain) where TResult : notnull
    {
        return result.Match(chain, static (chain, ok) => chain(ok.Value), static (_, er) => er);
    }

    public static Result<TResult> Chain<TInitial, TResult>(this Result<TInitial> result, Func<TInitial, Result<TResult>> chain)
        where TInitial : notnull
        where TResult : notnull
    {
        return result.Match(chain, static (chain, ok) => chain(ok.Value), static (_, er) => er.Problem);
    }

    public static Result<TResult> Chain<TInitial, TResult, TState>(this Result<TInitial> result, TState state, Func<TState, TInitial, Result<TResult>> chain)
        where TInitial : notnull
        where TResult : notnull
    {
        return result.Match((state, chain), static (tuple, ok) => tuple.chain(tuple.state, ok.Value), static (_, er) => er.Problem);
    }


    public static Result<TResult> ChainAsync<TResult>(this Result<TResult> result, Func<TResult, Result<TResult>> chain) where TResult : notnull
    {
        return result.Match(chain, static (chain, ok) => chain(ok.Value), static (_, er) => er);
    }

    public static Result<TResult> ChainAsync<TInitial, TResult>(this Result<TInitial> result, Func<TInitial, Result<TResult>> chain)
        where TInitial : notnull
        where TResult : notnull
    {
        return result.Match(chain, static (chain, ok) => chain(ok.Value), static (_, er) => er.Problem);
    }

    public static Result<TResult> ChainAsync<TInitial, TResult, TState>(this Result<TInitial> result, TState state, Func<TState, TInitial, Result<TResult>> chain)
        where TInitial : notnull
        where TResult : notnull
    {
        return result.Match((state, chain), static (tuple, ok) => tuple.chain(tuple.state, ok.Value), static (_, er) => er.Problem);
    }
}

public sealed class ResultJsonConverter : JsonConverterFactory
{
    private static readonly Type ResultType = typeof(Result<>);
    private static readonly Type converterType = typeof(Converter<>);

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition().IsAssignableFrom(ResultType)
            || typeToConvert.BaseType is { IsGenericType: true } baseType && baseType.GetGenericTypeDefinition().IsAssignableFrom(ResultType);
    }

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var type = typeToConvert.GetGenericArguments()[0];
        var converter = converterType.MakeGenericType(type);

        var instance = (JsonConverter)Activator.CreateInstance(converter)!;
        return instance;
    }

    /// <inheritdoc/>
    private class Converter<T> : JsonConverter<Result<T>>
        where T : notnull
    {
        private static readonly JsonEncodedText Result = JsonEncodedText.Encode("$result");
        private static readonly JsonEncodedText Value = JsonEncodedText.Encode("$value");
        private static readonly JsonEncodedText Ok = JsonEncodedText.Encode("ok");
        private static readonly JsonEncodedText Er = JsonEncodedText.Encode("er");

        /// <inheritdoc/>
        public override Result<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerAtStart = reader;
            // "$result" name check
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName ||
                !reader.ValueTextEquals(Result.EncodedUtf8Bytes))
            {
                throw new JsonException("Expected the first value to be a property named $result");
            }

            reader.Read();
            Result<T>? result;
            // "$result" value check (ok or er)
            if (reader.TokenType is JsonTokenType.String && reader.ValueTextEquals(Ok.EncodedUtf8Bytes))
            {
                reader.Read();
                // check if next type is property with name "$value"
                // if yes deserialize that object to T
                if (reader.TokenType is JsonTokenType.PropertyName && reader.ValueTextEquals(Value.EncodedUtf8Bytes))
                {
                    reader.Read();
                    var ok = JsonSerializer.Deserialize<T>(ref reader, options);
                    result = ok is { } ? ok
                        : new JsonException("Could not deserialize 'OK' $value because it was null.");
                }
                // it's the same object deserialize whole.
                else
                {
                    var ok = JsonSerializer.Deserialize<T>(ref readerAtStart, options);
                    result = ok is { } ? ok
                        : new JsonException("Could not deserialize 'OK' object because it was null.");
                }
            }
            else if (reader.TokenType is JsonTokenType.String && reader.ValueTextEquals(Er.EncodedUtf8Bytes))
            {
                var problem = JsonSerializer.Deserialize<Problem>(ref readerAtStart, options);
                result = problem is { } ? problem
                    : new JsonException("Could not deserialize 'ER' object because it was null.");
            }
            else
            {
                throw new JsonException();
            }
            while (reader.Read()) { }
            return result;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(Result, value.IsOk() ? Ok : Er);

            var json = value.Match(options,
                static (options, ok) => JsonSerializer.SerializeToElement(ok.Value, options),
                static (options, er) => JsonSerializer.SerializeToElement(er.Problem, options)
            );
            if (json.ValueKind is JsonValueKind.Object)
            {
                foreach (var obj in json.EnumerateObject())
                {
                    obj.WriteTo(writer);
                }
            }
            else
            {
                writer.WritePropertyName(Value);
                json.WriteTo(writer);
            }
            writer.WriteEndObject();
        }
    }
}
