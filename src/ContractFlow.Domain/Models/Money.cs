namespace ContractFlow.Domain.Models;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount deve ser >= 0");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length is < 3 or > 3)
            throw new ArgumentException("Currency deve ser código ISO-4217 (ex.: BRL, USD).", nameof(currency));

        Amount = decimal.Round(amount, 2, MidpointRounding.ToZero);
        Currency = currency.ToUpperInvariant();
    }

    public static implicit operator decimal(Money money) => money.Amount;
    public static implicit operator Money(decimal amount) => new(amount, "BRL");

    public static Money Of(decimal amount, string currency) => new(amount, currency);
}
