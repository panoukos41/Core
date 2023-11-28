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

[Union, JsonConverter(typeof(ResultJsonConverter))]
public abstract partial record Result<T> : IResultUnion where T : notnull
{
    public static implicit operator Result<T>(Exception ex) => new Er(ex);

    public static IResultUnion CreateOk<TOk>(TOk value) where TOk : notnull => new Result<TOk>.Ok(value);

    public static IResultUnion CreateEr(Problem problem) => new Result<T>.Er(problem);

    public partial record Ok(T Value);

    public partial record Er(Problem Problem);

    public bool IsOk() => this is Result<T>.Ok;

    public bool IsOk([NotNullWhen(true)] out T? value)
    {
        value = default;
        if (this is Result<T>.Ok ok)
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

        problem = ((Result<T>.Er)this).Problem;
        return false;
    }

    public bool IsEr() => this is Result<T>.Er;

    public bool IsEr([NotNullWhen(true)] out Problem? problem)
    {
        Unsafe.SkipInit(out problem);
        if (this is Result<T>.Er er)
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

        value = ((Result<T>.Ok)this).Value;
        return false;
    }

    public virtual bool Equals(Result<T>? other)
    {
        if (this is Result<T>.Ok ok &&
            other is Result<T>.Ok okOther)
        {
            return EqualityComparer<T>.Default.Equals(ok.Value, okOther.Value);
        }

        var er = (Result<T>.Er)this;
        var erOther = other as Result<T>.Er;

        return er.Problem == erOther?.Problem;
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
        static ok => ok.Value.GetHashCode(),
        static er => er.Problem.GetHashCode()
    );
}

public sealed class ResultJsonConverter : JsonConverterFactory
{
    private static readonly Type ResultType = typeof(Result<>);
    private static readonly Type converterType = typeof(Converter<>);

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType
            && typeToConvert.GetGenericTypeDefinition() == ResultType;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var type = typeToConvert.GetGenericArguments()[0];
        var converter = converterType.MakeGenericType(type);

        return (JsonConverter)Activator.CreateInstance(converter)!;
    }

    private class Converter<T> : JsonConverter<Result<T>>
        where T : notnull
    {
        private static readonly JsonEncodedText Result = JsonEncodedText.Encode("$result");
        private static readonly JsonEncodedText Value = JsonEncodedText.Encode("$value");
        private static readonly JsonEncodedText Ok = JsonEncodedText.Encode("ok");
        private static readonly JsonEncodedText Er = JsonEncodedText.Encode("er");

        public override Result<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerAtStart = reader;
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName ||
                !reader.ValueTextEquals(Result.EncodedUtf8Bytes))
            {
                throw new JsonException("Expected the first value to be a property named $result");
            }

            reader.Read();
            Result<T>? result;
            if (reader.ValueTextEquals(Ok.EncodedUtf8Bytes))
            {
                reader.Read();
                if (reader.ValueTextEquals(Value.EncodedUtf8Bytes))
                {
                    reader.Read();
                    var ok = JsonSerializer.Deserialize<T>(ref reader, options);
                    result = ok is { } ? ok
                        : new JsonException("Could not deserialize 'OK' $value because it was null.");
                }
                else
                {
                    var ok = JsonSerializer.Deserialize<T>(ref readerAtStart, options);
                    result = ok is { } ? ok
                        : new JsonException("Could not deserialize 'OK' object because it was null.");
                }
            }
            else if (reader.ValueTextEquals(Er.EncodedUtf8Bytes))
            {
                var er = JsonSerializer.Deserialize<Result<T>.Er>(ref readerAtStart, options);
                result = er is { } ? er
                    : new JsonException("Could not deserialize 'OK' object because it was null.");
            }
            else
            {
                throw new JsonException();
            }
            while (reader.Read()) { }
            return result;
        }

        public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
        {
            var ok = value is Result<T>.Ok;
            var e = ok
                ? JsonSerializer.SerializeToElement(((Result<T>.Ok)value).Value, options)
                : JsonSerializer.SerializeToElement((Result<T>.Er)value, options);

            writer.WriteStartObject();
            writer.WriteString(Result, ok ? Ok : Er);
            if (e.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
            {
                foreach (var obj in e.EnumerateObject())
                {
                    obj.WriteTo(writer);
                }
            }
            else
            {
                writer.WritePropertyName(Value);
                e.WriteTo(writer);
            }
            writer.WriteEndObject();
        }
    }
}
