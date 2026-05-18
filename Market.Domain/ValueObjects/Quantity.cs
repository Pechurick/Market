namespace Market.Domain.ValueObjects;

public record Quantity
{
    public int Value { get; init; }

    private Quantity(int value) => Value = value;

    public static Quantity Create(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Кількість товару має бути більшою за нуль.");
            
        if (value > 1000)
            throw new ArgumentException("Кількість товару перевищує допустимий ліміт.");

        return new Quantity(value);
    }

    public static implicit operator int(Quantity quantity) => quantity.Value;
}