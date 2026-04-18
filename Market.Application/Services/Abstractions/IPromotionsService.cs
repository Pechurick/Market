using Market.Application.Models.Requests;
using Market.Application.Models.Responses;

namespace Market.Application.Services.Abstractions;

public interface IPromotionsService
{
    Task<IEnumerable<PromotionDto>> GetAll(CancellationToken cancellationToken);
    Task<PromotionDto> Create(PromotionCreateDto request, CancellationToken cancellationToken);
}
