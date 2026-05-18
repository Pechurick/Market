namespace Market.Domain.Entities;

public class Promotion
{
    public Guid Id { get; set; }
    public required string Name { get; set; } 
    public required string Code { get; set; } 
    
    public decimal DiscountPercentage { get; set; } 
    
    public DateTime StartDate { get; set; } 
    public DateTime EndDate { get; set; }   

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
}
