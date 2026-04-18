using Market.Domain;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.Infrastructure.EntityTypeConfigurations;

public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
	public void Configure(EntityTypeBuilder<OrderItem> builder)
	{
		builder
			.HasKey(x => x.Id);
		
		builder
			.HasOne(x => x.Order)
			.WithMany(x => x.Items)
			.HasForeignKey(y => y.OrderId)
			.HasPrincipalKey(x => x.Id);
		
		builder
			.HasOne(x => x.Product)
			.WithMany(x => x.OrderItems)
			.HasForeignKey(y => y.ProductId)
			.HasPrincipalKey(x => x.Id);
	}
}
