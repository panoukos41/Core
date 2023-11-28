using System.Diagnostics.CodeAnalysis;

namespace Core.Commons;

public readonly record struct Money
{
    public required Currency Currency { get; init; }

    public required decimal Value { get; init; }

    public Money()
    {
        Currency = Currency.EUR;
        Value = 0;
    }

    [SetsRequiredMembers]
    public Money(Currency currency, decimal value)
    {
        Currency = currency;
        Value = value;
    }
}

public enum Currency
{
    EUR
}
