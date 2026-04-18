namespace Market.Application.Models.Requests;

public class OrderCreateDto
{
	public IEnumerable<OrderItemCreateDto> Items { get; set; } = [];
	// 👈 Нове поле для промокоду (необов'язкове)
    public string? PromoCode { get; set; }
}

public class OrderItemCreateDto
{
	public long ProductId { get; set; }
	public int Amount { get; set; }
}
