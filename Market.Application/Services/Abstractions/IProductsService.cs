using Market.Application.Models.Requests;
using Market.Application.Models.Responses;

namespace Market.Application.Services.Abstractions;

public interface IProductsService
{
	Task<IEnumerable<ProductListDto>> GetAll(Guid? categoryId, CancellationToken cancellationToken);
	Task<ProductListDto> Get(long id, CancellationToken cancellationToken);
	Task Create(ProductCreateDto request, CancellationToken cancellationToken);
	Task Update(long id, ProductUpdateDto request, CancellationToken cancellationToken);
	Task Delete(long id, CancellationToken cancellationToken);
	Task<PagedResultDto<ProductListDto>> GetPaged(Guid? categoryId, PagedRequestDto pagedRequest, CancellationToken cancellationToken);
}
