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
            .OnDelete(DeleteBehavior.Restrict); 
        
        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Orders)
            .HasPrincipalKey(x => x.Id)
            .HasForeignKey(x => x.UserId);

        
        builder.OwnsOne(x => x.TotalPrice, priceBuilder =>
        {
            priceBuilder.Property(p => p.Amount)
                .HasColumnName("Price") 
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            priceBuilder.Property(p => p.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .HasDefaultValue("UAH")
                .IsRequired();
        });
    }
}