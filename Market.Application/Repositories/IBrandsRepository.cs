using Market.Domain.Entities;

namespace Market.Application.Repositories;

public interface IBrandsRepository
{
    Task<IEnumerable<Brand>> GetAll(CancellationToken cancellationToken);
    Task<Brand?> Get(Guid id, CancellationToken cancellationToken);
    Task Add(Brand brand, CancellationToken cancellationToken);
    
    Task Update(Brand brand, CancellationToken cancellationToken);
    Task Delete(Brand brand, CancellationToken cancellationToken);
    Task<bool> HasProducts(Guid id, CancellationToken cancellationToken);
}
