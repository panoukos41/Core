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
public readonly struct None :
    IEquatable<None>,
    IEquatable<None?>,
    IComparable<None>,
    IComparable<None?>,
    IComparable
{
    /// <summary>
    /// Default and only value of the <see cref="None"/> type.
    /// </summary>
    public static readonly None Value = new();

    /// <summary>
    /// Task from a <see cref="None"/> type.
    /// </summary>
    public static readonly Task<None> Task = System.Threading.Tasks.Task.FromResult(Value);

    /// <summary>
    /// Value task with a <see cref="None"/> type.
    /// </summary>
    public static readonly ValueTask<None> ValueTask = new(Value);

    /// <summary>
    /// Determines whether the specified <see cref="object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) => obj is None;

    /// <summary>
    /// Determines whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <c>true</c> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(None other) => true;

    /// <summary>
    /// Determines whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <c>true</c> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(None? other) => other is { };

    /// <inheritdoc/>
    public int CompareTo(None other) => 0;

    /// <inheritdoc/>
    public int CompareTo(None? other) => other is { } ? 0 : -1;

    /// <inheritdoc/>
    int IComparable.CompareTo(object? obj) => obj is None ? 0 : -1;

    /// <inheritdoc/>
    public override int GetHashCode() => 0;

    /// <inheritdoc/>
    public static bool operator ==(None? first, None? second) => true;

    /// <inheritdoc/>
    public static bool operator !=(None? first, None? second) => false;

    /// <inheritdoc/>
    public static bool operator <(None? left, None? right) => false;

    /// <inheritdoc/>
    public static bool operator <=(None? left, None? right) => true;

    /// <inheritdoc/>
    public static bool operator >(None? left, None? right) => false;

    /// <inheritdoc/>
    public static bool operator >=(None? left, None? right) => true;

    /// <inheritdoc/>
    public static implicit operator Task(None? @void) => System.Threading.Tasks.Task.CompletedTask;

    /// <inheritdoc/>
    public static implicit operator ValueTask(None? @void) => System.Threading.Tasks.ValueTask.CompletedTask;

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
