using Market.Application.Repositories;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Repositories;

public class ProductsRepository(MarketDbContext context) : IProductsRepository
{
	public async Task<IEnumerable<Product>> GetAll(Guid? categoryId, CancellationToken cancellationToken) =>
		await context.Products
			.AsNoTracking()
			.Include(x => x.Category)
			.Include(x => x.Brand)
			.Where(x => (!categoryId.HasValue || x.CategoryId == categoryId) && !x.IsDeleted)
			.ToListAsync(cancellationToken);
	
	public async Task<Product?> Get(long id, CancellationToken cancellationToken) =>
    await context.Products
        .Include(x => x.Category)
		.Include(x => x.Brand)
        .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

	public async Task<IEnumerable<Product>> Get(IEnumerable<long> ids, CancellationToken cancellationToken) =>
		await context.Products
			.Where(x => ids.Contains(x.Id))
			.ToListAsync(cancellationToken);

	public async Task<bool> IsProductInCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        return await context.Products
            .IgnoreQueryFilters() 
            .AnyAsync(x => x.CategoryId == categoryId, cancellationToken);
    }

	public async Task Add(Product product, CancellationToken cancellationToken)
	{
		await context.Products.AddAsync(product, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
	}
	
	public async Task Update(Product product, CancellationToken cancellationToken)
	{
		context.Products.Update(product);
		await context.SaveChangesAsync(cancellationToken);
	}
	
	public async Task UpdateRange(IEnumerable<Product> products, CancellationToken cancellationToken)
	{
		context.Products.UpdateRange(products);
		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPaged(
    Guid? categoryId, 
    int pageNumber, 
    int pageSize, 
    CancellationToken cancellationToken)
	{
    var query = context.Products
        .AsNoTracking()
        .Include(x => x.Category) 
        .Include(x => x.Brand)    
        .Where(x => !categoryId.HasValue || x.CategoryId == categoryId);

    var totalCount = await query.CountAsync(cancellationToken);

    var items = await query
        .OrderBy(x => x.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return (items, totalCount);
	}
}
