using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;

namespace Market.Application.Services;

public class PromotionsService(IPromotionsRepository repository) : IPromotionsService
{
    public async Task<IEnumerable<PromotionDto>> GetAll(CancellationToken cancellationToken)
    {
        var promotions = await repository.GetAll(cancellationToken);
        return promotions.Select(p => new PromotionDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            DiscountPercentage = p.DiscountPercentage,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            CategoryId = p.CategoryId
        });
    }

    public async Task<PromotionDto> Create(PromotionCreateDto request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByCode(request.Code, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException($"Промокод '{request.Code}' вже існує.");
        }

        var promotion = new Promotion
        {
            Name = request.Name,
            Code = request.Code.ToUpper(),
            DiscountPercentage = request.DiscountPercentage,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CategoryId = request.CategoryId
        };

        await repository.Add(promotion, cancellationToken);

        return new PromotionDto
        {
            Id = promotion.Id,
            Name = promotion.Name,
            Code = promotion.Code,
            DiscountPercentage = promotion.DiscountPercentage,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate,
            CategoryId = promotion.CategoryId
        };
    }
}