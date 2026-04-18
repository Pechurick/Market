using Market.Application.Models.Requests;
using Market.Application.Models.Responses;

namespace Market.Application.Services.Abstractions;

public interface IBrandsService
{
    Task<IEnumerable<BrandDto>> GetAll(CancellationToken cancellationToken);
    Task Create(BrandCreateDto request, CancellationToken cancellationToken);
    Task<BrandDto?> GetById(Guid id, CancellationToken cancellationToken);
    Task Update(Guid id, BrandUpdateDto request, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
}
