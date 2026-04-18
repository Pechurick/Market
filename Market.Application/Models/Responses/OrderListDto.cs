using Market.Domain.Enums;

namespace Market.Application.Models.Responses;

public class OrderListDto
{
	public long Id { get; set; }
	public decimal Price { get; set; }
	public DateTime CreatedAt { get; set; }
	public OrderStatus Status { get; set; }
}
