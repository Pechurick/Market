using Market.Application.Repositories;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Repositories;

public class BrandsRepository(MarketDbContext context) : IBrandsRepository
{
    public async Task<IEnumerable<Brand>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Brands.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Brand?> Get(Guid id, CancellationToken cancellationToken)
    {
        return await context.Brands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task Add(Brand brand, CancellationToken cancellationToken)
    {
        await context.Brands.AddAsync(brand, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    // Додай ці методи всередину класу BrandsRepository
    public async Task Update(Brand brand, CancellationToken cancellationToken)
    {
        context.Brands.Update(brand);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(Brand brand, CancellationToken cancellationToken)
    {
        context.Brands.Remove(brand);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasProducts(Guid id, CancellationToken cancellationToken)
    {
        return await context.Products.AnyAsync(p => p.BrandId == id, cancellationToken);
    }
}
