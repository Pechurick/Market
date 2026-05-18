using Market.Application.Repositories;
using Market.Domain;
using Market.Domain.Entities;
using Market.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Repositories;

public class OrdersRepository(MarketDbContext context) : IOrdersRepository
{
    public async Task<IEnumerable<Order>> GetAll(long userId, CancellationToken cancellationToken) =>
        await context.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    
    public async Task<Order?> Get(long id, long userId, CancellationToken cancellationToken) =>
        await context.Orders
            .Include(x => x.Items)

            .ThenInclude(x => x.Product) 
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task Add(Order order, CancellationToken cancellationToken)
    {
        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task Update(Order order, CancellationToken cancellationToken)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalSpentByUser(long userId, CancellationToken ct) 
    {
        return await context.Orders
            .Where(o => o.UserId == userId && o.Status == OrderStatus.Delivered)
           
            .SumAsync(o => o.TotalPrice.Amount, ct); 
    }

    public async Task<(IEnumerable<Order> Items, int TotalCount)> GetPaged(
    long userId,
    int pageNumber, 
    int pageSize, 
    CancellationToken cancellationToken)
    {
        var query = context.Orders
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        
        var totalCount = await query.CountAsync(cancellationToken);

        
        var items = await query
            .OrderByDescending(x => x.CreatedAt) 
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}