using Market.Domain;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.Infrastructure.EntityTypeConfigurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder
			.HasIndex(x => x.Id);

		builder
			.Property(x => x.FirstName)
			.HasMaxLength(100);
		
		builder
			.Property(x => x.LastName)
			.HasMaxLength(100);
		
		builder
			.Property(x => x.MiddleName)
			.HasMaxLength(100);
		
		builder
			.HasMany(x => x.Orders)
			.WithOne(x => x.User)
			.HasForeignKey(y => y.UserId)
			.HasPrincipalKey(x => x.Id);
	}
}
