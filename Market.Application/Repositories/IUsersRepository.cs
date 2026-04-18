using Market.Domain.Entities;

namespace Market.Application.Repositories;

public interface IUsersRepository
{
	Task<IEnumerable<User>> GetAll(CancellationToken cancellationToken);
	Task<User?> Get(long id, CancellationToken cancellationToken);
	Task Add(User user, CancellationToken cancellationToken);
	Task Update(User user, CancellationToken cancellationToken);
	Task Delete(User user, CancellationToken cancellationToken);
}
