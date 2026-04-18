namespace Market.Domain.Entities;

public class OrderItem
{
	public long Id { get; set; }
	public long OrderId { get; set; }
	public long ProductId { get; set; }
	public int Amount { get; set; }
	public decimal Price { get; set; }

	public Order? Order { get; set; }
	public Product? Product { get; set; }
}
