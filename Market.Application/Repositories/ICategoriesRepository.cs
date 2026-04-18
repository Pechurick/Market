using Market.Domain.Entities;

namespace Market.Application.Repositories;

public interface ICategoriesRepository
{
	Task<IEnumerable<Category>> GetAll(CancellationToken cancellationToken);
	Task<Category?> Get(Guid id, CancellationToken cancellationToken);
	Task Add(Category category, CancellationToken cancellationToken);
	Task Update(Category category, CancellationToken cancellationToken);
	Task Delete(Category category, CancellationToken cancellationToken);
	Task<bool> HasSubCategories(Guid id, CancellationToken cancellationToken);
}
