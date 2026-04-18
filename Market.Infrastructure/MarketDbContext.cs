using System.Reflection;
using Market.Domain;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure;

public class MarketDbContext(DbContextOptions<MarketDbContext> options) : DbContext(options)
{
	public DbSet<Category> Categories { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<Brand> Brands { get; set; }
	public DbSet<Promotion> Promotions { get; set; }
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Promotion>().ToTable("Promotion");

		// --- НОВИЙ БЛОК: Global Query Filters ---
        
        // Фільтр для користувачів: завжди ігнорувати тих, у кого IsDeleted = true
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

        // Якщо в тебе є поле IsDeleted і для продуктів, можеш розкоментувати і цей рядок:
        // modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        
        // ----------------------------------------
	}
}
