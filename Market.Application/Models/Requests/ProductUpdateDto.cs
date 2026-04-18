namespace Market.Application.Models.Requests;

public class ProductUpdateDto
{
	public required string Name { get; set; }
	public Guid CategoryId { get; set; }
	public int? Amount { get; set; }
	public decimal? Price { get; set; }

	public Guid? BrandId { get; set; } // Щоб при створенні миші можна було вказати Razer
}
