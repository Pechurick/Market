using Market.Domain.Entities;

namespace Market.Application.Repositories;

public interface IProductsRepository
{
	Task<IEnumerable<Product>> GetAll(Guid? categoryId, CancellationToken cancellationToken);
	Task<Product?> Get(long id, CancellationToken cancellationToken);
	Task<IEnumerable<Product>> Get(IEnumerable<long> ids, CancellationToken cancellationToken);
	Task<bool> IsProductInCategory(Guid categoryId, CancellationToken cancellationToken);
	Task Add(Product product, CancellationToken cancellationToken);
	Task Update(Product product, CancellationToken cancellationToken);
	Task UpdateRange(IEnumerable<Product> products, CancellationToken cancellationToken);
	Task<(IEnumerable<Product> Items, int TotalCount)> GetPaged(Guid? categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}
