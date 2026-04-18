namespace Market.Domain.Entities;

public class Promotion
{
    public Guid Id { get; set; }
    public required string Name { get; set; } // Наприклад: "Чорна п'ятниця на мікрофони"
    public required string Code { get; set; } // Наприклад: "STREAM2026"
    
    public decimal DiscountPercentage { get; set; } // Відсоток знижки (наприклад, 15.0)
    
    public DateTime StartDate { get; set; } // Коли починає діяти 
    public DateTime EndDate { get; set; }   // Коли закінчує діяти 

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
}
