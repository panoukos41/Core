namespace Core;

// Unit code from: https://github.com/jbogard/MediatR/blob/master/src/MediatR.Contracts/Unit.cs

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

/// <summary>
/// Represents a void type, since <see cref="void"/> is not a valid return type in C#.
/// </summary>
[JsonConverter(typeof(NoneJsonConverter))]
[Serializable]
public sealed class None : IEquatable<None>
{
    /// <summary>
    /// Default and only value of the <see cref="None"/> type.
    /// </summary>
    public static readonly None Value = new();

    /// <summary>
    /// Task with a <see cref="None"/> return type.
    /// </summary>
    public static readonly Task<None> Task = System.Threading.Tasks.Task.FromResult(Value);

    /// <summary>
    /// Value task with a <see cref="None"/> return type.
    /// </summary>
    public static readonly ValueTask<None> ValueTask = new(Value);

    /// <inheritdoc/>
    public bool Equals(None? other) => other is { };

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is None;

    /// <inheritdoc/>
    public override int GetHashCode() => 0;

    /// <inheritdoc/>
    public override string ToString() => "{}";

    public static bool operator ==(None first, None second) => true;

    public static bool operator !=(None first, None second) => false;

    public static implicit operator Task(None none) => System.Threading.Tasks.Task.CompletedTask;

    public static implicit operator Task<None>(None none) => Task;

    public static implicit operator ValueTask(None none) => System.Threading.Tasks.ValueTask.CompletedTask;

    public static implicit operator ValueTask<None>(None none) => ValueTask;

    private sealed class NoneJsonConverter : JsonConverter<None>
    {
        public override None Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Value;
        }

        public override void Write(Utf8JsonWriter writer, None value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
    }
}
