namespace Market.Domain.Entities;

public class Product
{
	public long Id { get; set; }
	public required string Name { get; set; }
	public Guid CategoryId { get; set; }
	public int Amount { get; set; }
	public decimal Price { get; set; }
	public bool IsDeleted { get; set; }

	
    public Guid? BrandId { get; set; }
    public Brand? Brand { get; set; }

	public Category? Category { get; set; }
	public ICollection<OrderItem> OrderItems { get; set; } = [];

	public int ReservedAmount { get; set; } = 0;
}
