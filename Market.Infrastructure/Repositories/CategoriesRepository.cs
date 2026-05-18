using Market.Application.Repositories;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Repositories;

public class CategoriesRepository(MarketDbContext context) : ICategoriesRepository
{
	public async Task<IEnumerable<Category>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Categories.ToListAsync(cancellationToken);
    }
	
	public async Task<Category?> Get(Guid id, CancellationToken cancellationToken) =>
        await context.Categories
            .Include(c => c.SubCategories) 
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

	public async Task Add(Category category, CancellationToken cancellationToken)
	{
		await context.Categories.AddAsync(category, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
	}
	
	public async Task Update(Category category, CancellationToken cancellationToken)
	{
		context.Categories.Update(category);
		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task<bool> HasSubCategories(Guid id, CancellationToken cancellationToken)
	{
    	return await context.Categories.AnyAsync(c => c.ParentId == id, cancellationToken);
	}
	
	public async Task Delete(Category category, CancellationToken cancellationToken)
	{
		context.Categories.Remove(category);
		await context.SaveChangesAsync(cancellationToken);
	}
}
