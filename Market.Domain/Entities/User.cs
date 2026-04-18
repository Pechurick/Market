using Market.Domain.Enums;

namespace Market.Domain.Entities;

public class User
{
	public long Id { get; set; }
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public string? MiddleName { get; set; }
	public DateOnly? BirthDate { get; set; }
	public bool IsDeleted { get; set; }

	public ICollection<Order> Orders { get; set; } = [];

	// Рівень лояльності клієнта (за замовчуванням Standard)
    public LoyaltyTier LoyaltyTier { get; set; } = LoyaltyTier.Standard;

    // Сума всіх покупок (щоб знати, коли переводити його на Silver чи Gold)
    public decimal TotalSpent { get; set; } = 0;
}
