using Market.Domain;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.Infrastructure.EntityTypeConfigurations;

public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .HasIndex(x => x.Id);

        builder
            .Property(x => x.Name)
            .HasMaxLength(10_000);

        builder
            .HasMany(x => x.Products)
            .WithOne(x => x.Category)
            .HasPrincipalKey(x => x.Id)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // 🛡️ ВАЖЛИВО: Забороняємо каскадне видалення товарів!;
    }
}
