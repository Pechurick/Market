using Market.Domain;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.Infrastructure.EntityTypeConfigurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder
			.HasIndex(x => x.Id);

		builder
			.Property(x => x.Name)
			.HasMaxLength(10_000);
		
		builder
			.HasOne(x => x.Category)
			.WithMany(x => x.Products)
			.HasForeignKey(y => y.CategoryId)
			.HasPrincipalKey(x => x.Id);
		
		builder
			.HasMany(x => x.OrderItems)
			.WithOne(x => x.Product)
			.HasForeignKey(x => x.ProductId)
			.HasPrincipalKey(x => x.Id);

		// Зв'язок Товар -> Бренд
        builder
            .HasOne(x => x.Brand)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.SetNull); // Якщо ми видалимо бренд Razer, товари не зникнуть, просто їхній BrandId стане null
	}
}
