using Market.Application.Models.Requests;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;
using Market.Domain.Enums;

namespace Market.Application.Services;

public class PricingEngine(
    IUsersRepository usersRepository,
    IPromotionsRepository promotionsRepository,
    TimeProvider timeProvider) : IPricingEngine
{
    
    public async Task<(decimal FinalPrice, Guid? PromoId)> CalculateFinalPriceAsync(
        long userId, 
        List<Product> products, 
        IEnumerable<OrderItemCreateDto> items, 
        string? promoCode, 
        CancellationToken cancellationToken)
    {
        
        var user = await usersRepository.Get(userId, cancellationToken);
        var promotion = string.IsNullOrWhiteSpace(promoCode) ? null : await GetValidPromotionOrThrow(promoCode, cancellationToken);

        
        decimal basePrice = CalculateBasePrice(products, items);
        decimal currentPrice = basePrice;
        int totalItemsCount = items.Sum(x => x.Amount);

        
        currentPrice = ApplyBulkDiscount(currentPrice, totalItemsCount);
        currentPrice = ApplyLoyaltyDiscount(currentPrice, user?.LoyaltyTier);
        currentPrice = ApplyPromotionDiscount(currentPrice, basePrice, products, items, promotion);

        return (Math.Round(currentPrice, 2), promotion?.Id);
    }

    

    private decimal CalculateBasePrice(List<Product> products, IEnumerable<OrderItemCreateDto> items) =>
        products.Sum(p => p.Price * items.First(i => i.ProductId == p.Id).Amount);

    private decimal ApplyBulkDiscount(decimal currentPrice, int totalItemsCount) =>
        totalItemsCount >= 5 ? currentPrice * 0.95m : currentPrice;

    private decimal ApplyLoyaltyDiscount(decimal currentPrice, LoyaltyTier? tier)
    {
        decimal discount = tier switch
        {
            LoyaltyTier.Bronze => 0.03m,
            LoyaltyTier.Silver => 0.05m,
            LoyaltyTier.Gold => 0.10m,
            _ => 0m
        };
        return currentPrice - (currentPrice * discount);
    }

    private decimal ApplyPromotionDiscount(
        decimal currentPrice, 
        decimal baseTotalPrice, 
        List<Product> products, 
        IEnumerable<OrderItemCreateDto> items, 
        Promotion? promo)
    {
        if (promo == null) return currentPrice;

        if (!promo.CategoryId.HasValue)
        {
            return currentPrice - (currentPrice * (promo.DiscountPercentage / 100m));
        }

        
        decimal baseCategoryPrice = products
            .Where(p => p.CategoryId == promo.CategoryId.Value)
            .Sum(p => p.Price * items.First(i => i.ProductId == p.Id).Amount);

        decimal currentCategoryPrice = baseTotalPrice > 0 
            ? baseCategoryPrice * (currentPrice / baseTotalPrice) 
            : 0;

        decimal discountAmount = currentCategoryPrice * (promo.DiscountPercentage / 100m);
        return currentPrice - discountAmount;
    }

    

    private async Task<Promotion> GetValidPromotionOrThrow(string promoCode, CancellationToken ct)
    {
        var promotion = await promotionsRepository.GetByCode(promoCode, ct);
        var now = timeProvider.GetLocalNow().DateTime;

        if (promotion == null || promotion.StartDate > now || promotion.EndDate < now)
        {
            throw new InvalidOperationException($"Промокод '{promoCode}' недійсний або прострочений.");
        }

        return promotion;
    }
}