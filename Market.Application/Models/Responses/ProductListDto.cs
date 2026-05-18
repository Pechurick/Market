namespace Market.Application.Models.Responses;

public class ProductListDto
{
	public long Id { get; set; }
	public required string Name { get; set; }
	public required ProductListCategoryDto Category { get; set; }
	public int Amount { get; set; }
	public decimal Price { get; set; }

	public Guid? BrandId { get; set; }
    public string? BrandName { get; set; } 
}

public class ProductListCategoryDto
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
}
