using Market.Application.Models.Requests;
using Market.Domain.Entities;

namespace Market.Application.Services.Abstractions;

public interface IPricingEngine
{
    Task<(decimal FinalPrice, Guid? PromoId)> CalculateFinalPriceAsync(
        long userId, 
        List<Product> products, 
        IEnumerable<OrderItemCreateDto> items, 
        string? promoCode, 
        CancellationToken cancellationToken);
}