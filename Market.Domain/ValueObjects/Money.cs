namespace Market.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "UAH")
    {
        if (amount < 0) throw new ArgumentException("Сума не може бути від'ємною");
        return new Money(amount, currency);
    }
}