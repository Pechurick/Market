using Market.Domain;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.Infrastructure.EntityTypeConfigurations;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder
			.HasIndex(x => x.Id);

		builder
			.HasMany(x => x.Items)
			.WithOne(x => x.Order)
			.HasPrincipalKey(x => x.Id)
			.HasForeignKey(x => x.OrderId)
			.OnDelete(DeleteBehavior.Restrict); // 🛡️ ВАЖЛИВО: Забороняємо каскадне видалення позицій із чека!;
		
		builder
			.HasOne(x => x.User)
			.WithMany(x => x.Orders)
			.HasPrincipalKey(x => x.Id)
			.HasForeignKey(x => x.UserId);
	}
}
