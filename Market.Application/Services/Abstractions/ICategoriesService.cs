using Market.Application.Models.Requests;
using Market.Application.Models.Responses;

namespace Market.Application.Services.Abstractions;

public interface ICategoriesService
{
	Task<IEnumerable<CategoryListDto>> GetAll(CancellationToken cancellationToken);
	Task<CategoryListDto?> GetById(Guid id, CancellationToken cancellationToken);
	Task Create(CategoryCreateDto request, CancellationToken cancellationToken);
	Task Update(Guid id, CategoryUpdateDto request, CancellationToken cancellationToken);
	Task Delete(Guid id, CancellationToken cancellationToken);
}
