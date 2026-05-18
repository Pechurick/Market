namespace Market.Application.Models.Requests;

public class ProductCreateDto
{
	public required string Name { get; set; }
	public Guid CategoryId { get; set; }
	public int? Amount { get; set; }
	public decimal? Price { get; set; }

	public Guid? BrandId { get; set; } 
}
