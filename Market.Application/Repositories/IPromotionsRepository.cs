using Market.Domain.Entities;

namespace Market.Application.Repositories;

public interface IPromotionsRepository
{
    Task<IEnumerable<Promotion>> GetAll(CancellationToken cancellationToken);
    Task<Promotion?> GetById(Guid id, CancellationToken cancellationToken); 
    Task<Promotion?> GetByCode(string code, CancellationToken cancellationToken);
    Task Add(Promotion promotion, CancellationToken cancellationToken);
    Task Update(Promotion promotion, CancellationToken cancellationToken);
}
