using Market.Application.Repositories;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Repositories;

public class UsersRepository(MarketDbContext context) : IUsersRepository
{
	public async Task<IEnumerable<User>> GetAll(CancellationToken cancellationToken) =>
		await context.Users
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	
	public async Task<User?> Get(long id, CancellationToken cancellationToken) =>
		await context.Users
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

	public async Task Add(User user, CancellationToken cancellationToken)
	{
		await context.Users.AddAsync(user, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
	}
	
	public async Task Update(User user, CancellationToken cancellationToken)
	{
		context.Users.Update(user);
		await context.SaveChangesAsync(cancellationToken);
	}
	
	public async Task Delete(User user, CancellationToken cancellationToken)
    {
        
        user.IsDeleted = true;
        
        context.Users.Update(user); 
        await context.SaveChangesAsync(cancellationToken);
    }
}
