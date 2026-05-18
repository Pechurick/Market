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

	
    public LoyaltyTier LoyaltyTier { get; set; } = LoyaltyTier.Standard;

    
    public decimal TotalSpent { get; set; } = 0;
}
