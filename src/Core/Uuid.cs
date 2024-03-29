﻿using Core.Common.JsonConverters;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Core;

/// <summary>
/// Represents a universally unique identifier using <see href="https://github.com/codeyu/nanoid-net">NanoId</see>.
/// A <see cref="Uuid"/> can be between 11 and 32 characters.
/// </summary>
[JsonConverter(typeof(UuidJsonConverter))]
[DebuggerDisplay("{ToString()}")]
public readonly struct Uuid :
    IParsable<Uuid>,
    IEquatable<Uuid>
{
    public static Uuid Empty { get; }

    private readonly string? nanoId;

    private Uuid(string nanoId)
    {
        this.nanoId = nanoId;
    }

    /// <summary>
    /// Get a new <see cref="Uuid"/> with the default <paramref name="size"/> of 11.
    /// </summary>
    /// <param name="size">The size of the id.</param>
    /// <remarks>Size has to be between 11 and 32 characters.</remarks>
    /// <returns>A new <see cref="Uuid"/>.</returns>
    public static Uuid NewUuid(int size = 11)
    {
        if (size < 11)
            throw new ArgumentOutOfRangeException(nameof(size), "Uuid can't be less than 11 characters.");
        if (size > 32)
            throw new ArgumentOutOfRangeException(nameof(size), "Uuid can't be more than 32 characters.");

        return new(Nanoid.Generate(size: size));
    }

    public override string ToString()
    {
        return nanoId ?? string.Empty;
    }

    #region IParsable

    public static Uuid Parse(string s, IFormatProvider? provider)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException("Uuid can't be null, empty or whitespace.", nameof(s));
        if (s.Length < 11)
            throw new ArgumentOutOfRangeException(nameof(s), "Uuid can't be less than 11 characters.");
        if (s.Length > 32)
            throw new ArgumentOutOfRangeException(nameof(s), "Uuid can't be more than 32 characters.");

        return new Uuid(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Uuid result)
    {
        result = Empty;
        if (s is not { Length: >= 11 or <= 32 })
            return false;

        result = Parse(s, provider);
        return true;
    }

    public static Uuid Parse(string s)
        => Parse(s, null);

    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out Uuid result)
        => TryParse(s, null, out result);

    #endregion

    #region Equals

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Uuid other && nanoId == other.nanoId;

    public bool Equals(Uuid other) => nanoId == other.nanoId;

    public override int GetHashCode() => HashCode.Combine(nanoId);

    public static bool operator ==(Uuid left, Uuid right) => left.nanoId == right.nanoId;

    public static bool operator !=(Uuid left, Uuid right) => left.nanoId != right.nanoId;

    #endregion

    public static implicit operator string(Uuid value) => value.ToString();
}

// MIT License
// 
// Copyright(c) 2017 zhu yu
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

file static class Nanoid
{
    // "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    // Must contain between 1 and 255 symbols.
    private const string alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly CryptoRandom Random = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="random"></param>
    /// <param name="alphabet"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string Generate(int size = 21)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "size must be greater than zero.");
        }

        // See https://github.com/ai/nanoid/blob/master/format.js for
        // explanation why masking is use (`random % alphabet` is a common
        // mistake security-wise).
        var mask = (2 << 31 - Clz32((alphabet.Length - 1) | 1)) - 1;
        var step = (int)Math.Ceiling(1.6 * mask * size / alphabet.Length);

        Span<char> idBuilder = stackalloc char[size];
        Span<byte> bytes = stackalloc byte[step];
        int cnt = 0;
        while (true)
        {
            Random.NextBytes(bytes);

            for (var i = 0; i < step; i++)
            {
                var alphabetIndex = bytes[i] & mask;

                if (alphabetIndex >= alphabet.Length) continue;

                idBuilder[cnt] = alphabet[alphabetIndex];
                if (++cnt == size)
                {
                    return new string(idBuilder);
                }
            }
        }
    }

    /// <summary>
    /// Counts leading zeros of <paramref name="x"/>.
    /// </summary>
    /// <param name="x">Input number.</param>
    /// <returns>Number of leading zeros.</returns>
    /// <remarks>
    /// Courtesy of spender/Sunsetquest see https://stackoverflow.com/a/10439333/623392.
    /// </remarks>
    internal static int Clz32(int x)
    {
        const int numIntBits = sizeof(int) * 8; //compile time constant
        //do the smearing
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        //count the ones
        x -= x >> 1 & 0x55555555;
        x = (x >> 2 & 0x33333333) + (x & 0x33333333);
        x = (x >> 4) + x & 0x0f0f0f0f;
        x += x >> 8;
        x += x >> 16;
        return numIntBits - (x & 0x0000003f); //subtract # of 1s from 32
    }
}

/// <inheritdoc />
/// <summary>
/// </summary>
file class CryptoRandom : Random
{
    private static readonly RandomNumberGenerator _r;
    private readonly byte[] _uint32Buffer = new byte[4];

    static CryptoRandom()
    {
        _r = RandomNumberGenerator.Create();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public override void NextBytes(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        _r.GetBytes(buffer);
    }

    /// <inheritdoc/>
    public override void NextBytes(Span<byte> buffer)
    {
        RandomNumberGenerator.Fill(buffer);
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override double NextDouble()
    {
        Span<byte> uint32Buffer = stackalloc byte[4];
        RandomNumberGenerator.Fill(uint32Buffer);
        return BitConverter.ToUInt32(uint32Buffer) / (1.0 + uint.MaxValue);
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
    public override int Next(int minValue, int maxValue)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minValue, maxValue);
        if (minValue == maxValue) return minValue;
        var range = (long)maxValue - minValue;
        return (int)((long)Math.Floor(NextDouble() * range) + minValue);
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override int Next()
    {
        return Next(0, int.MaxValue);
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
    public override int Next(int maxValue)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxValue);
        return Next(0, maxValue);
    }
}
