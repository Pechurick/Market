using Market.Domain.Enums;

namespace Market.Application.Models.Responses;

public class OrderDto
{
	public long Id { get; set; }
	public decimal Price { get; set; }
	public DateTime CreatedAt { get; set; }
	public OrderStatus Status { get; set; }
	public IEnumerable<OrderItemDto> Items { get; set; } = [];
}

public class OrderItemDto
{
	public long Id { get; set; }
	public required OrderItemProductDto Product { get; set; }
	public int Amount { get; set; }
	public decimal Price { get; set; }
}

public class OrderItemProductDto
{
	public long Id { get; set; }
	public required string Name { get; set; }
}
