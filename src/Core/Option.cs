using Dunet;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core;

[Union]
[JsonConverter(typeof(OptionJsonConverter))]
public abstract partial record Option<T> where T : notnull
{
    [JsonConverter(typeof(OptionJsonConverter))]
    public partial record Some(T Value);

    [JsonConverter(typeof(OptionJsonConverter))]
    public partial record None();

    public bool IsSome() => this is Some;

    public bool IsSome([NotNullWhen(true)] out T? value)
    {
        value = default;
        if (this is Some Some)
        {
            value = Some.Value;
            return true;
        }
        return false;
    }

    public bool IsNone() => this is None;

    public bool IsNone([NotNullWhen(false)] out T? value)
    {
        Unsafe.SkipInit(out value);
        if (IsNone()) return true;

        value = ((Some)this).Value;
        return false;
    }

    public virtual bool Equals(Option<T>? other)
    {
        if (this is Some Some &&
            other is Some SomeOther)
        {
            return EqualityComparer<T>.Default.Equals(Some.Value, SomeOther.Value);
        }

        return this is None && other is None;
    }

    public virtual bool Equals(T? other)
    {
        return other is { } && IsSome(out var value) && EqualityComparer<T>.Default.Equals(value, other);
    }

    public override int GetHashCode() => Match(
        static Some => HashCode.Combine(Some.Value),
        static er => HashCode.Combine(typeof(None))
    );

    public static implicit operator Option<T>(T? value) => value is { } ? new Some(value) : new None();
}

public sealed class OptionJsonConverter : JsonConverterFactory
{
    private static readonly Type OptionType = typeof(Option<>);
    private static readonly Type converterType = typeof(Converter<>);

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition().IsAssignableFrom(OptionType)
            || typeToConvert.BaseType is { IsGenericType: true } baseType && baseType.GetGenericTypeDefinition().IsAssignableFrom(OptionType);
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
    private class Converter<T> : JsonConverter<Option<T>>
        where T : notnull
    {
        private static readonly JsonEncodedText Option = JsonEncodedText.Encode("$option");
        private static readonly JsonEncodedText Value = JsonEncodedText.Encode("$value");
        private static readonly JsonEncodedText Some = JsonEncodedText.Encode("some");
        private static readonly JsonEncodedText None = JsonEncodedText.Encode("none");

        private static readonly Option<T> CachedNone = new Option<T>.None();
        private static readonly JsonDocument CachedNoneJson = JsonDocument.Parse("{}");

        /// <inheritdoc/>
        public override Option<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerAtStart = reader;
            // "$Option" name check
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName ||
                !reader.ValueTextEquals(Option.EncodedUtf8Bytes))
            {
                throw new JsonException("Expected the first value to be a property named $Option");
            }

            reader.Read();
            Option<T>? option;
            // "$Option" value check (Some or er)
            if (reader.TokenType is JsonTokenType.String && reader.ValueTextEquals(Some.EncodedUtf8Bytes))
            {
                reader.Read();
                // check if next type is property with name "$value"
                // if yes deserialize that object to T
                if (reader.TokenType is JsonTokenType.PropertyName && reader.ValueTextEquals(Value.EncodedUtf8Bytes))
                {
                    reader.Read();
                    var some = JsonSerializer.Deserialize<T>(ref reader, options);
                    option = some ?? throw new JsonException("Could not deserialize 'Some' $value because it was null.");
                }
                // it's the same object deserialize whole.
                else
                {
                    var some = JsonSerializer.Deserialize<T>(ref readerAtStart, options);
                    option = some ?? throw new JsonException("Could not deserialize 'Some' object because it was null.");
                }
            }
            else if (reader.TokenType is JsonTokenType.String && reader.ValueTextEquals(None.EncodedUtf8Bytes))
            {
                option = CachedNone;
            }
            else
            {
                throw new JsonException();
            }
            while (reader.Read()) { }
            return option;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(Option, value.IsSome() ? Some : None);

            var json = value.Match(options,
                static (options, ok) => JsonSerializer.SerializeToElement(ok.Value, options),
                static (options, er) => CachedNoneJson.RootElement
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
