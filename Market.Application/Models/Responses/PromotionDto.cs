namespace Market.Application.Models.Responses;

public class PromotionDto
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? CategoryId { get; set; } 
}
