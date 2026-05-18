using Market.Application.Repositories;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Repositories;

public class PromotionsRepository(MarketDbContext context) : IPromotionsRepository
{
    public async Task<IEnumerable<Promotion>> GetAll(CancellationToken cancellationToken) =>
        await context.Promotions.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Promotion?> GetById(Guid id, CancellationToken cancellationToken) => 
        await context.Promotions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Promotion?> GetByCode(string code, CancellationToken cancellationToken) =>
        await context.Promotions.FirstOrDefaultAsync(p => p.Code.ToLower() == code.ToLower(), cancellationToken);

    public async Task Add(Promotion promotion, CancellationToken cancellationToken)
    {
        await context.Promotions.AddAsync(promotion, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Promotion promotion, CancellationToken cancellationToken)
    {
        context.Promotions.Update(promotion);
        await context.SaveChangesAsync(cancellationToken);
    }
}
