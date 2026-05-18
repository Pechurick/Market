using Market.Domain;
using Market.Domain.Entities;
using Market.Domain.ValueObjects; 
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

        
        builder.OwnsOne(x => x.Price, priceBuilder =>
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


        builder.Property(oi => oi.Quantity)
            .HasConversion(
                q => q.Value,                    
                v => Quantity.Create(v))         
            .HasColumnName("Quantity")
            .IsRequired();
    }
}