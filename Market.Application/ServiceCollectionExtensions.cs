using Market.Application.Services;
using Market.Application.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Market.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoriesService, CategoriesService>();
        services.AddScoped<IOrdersService, OrdersService>();
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBrandsService, BrandsService>();
        services.AddScoped<IPromotionsService, PromotionsService>();
        services.AddScoped<IPricingEngine, PricingEngine>();
        services.AddScoped<ILoyaltyService, LoyaltyService>();
        services.AddScoped<IInventoryService, InventoryService>();
        
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        
        // 🪄 Даємо компілятору його Action, а всередині кажемо просканувати нашу збірку
		services.AddAutoMapper(cfg => 
		{
    		cfg.AddMaps(typeof(ServiceCollectionExtensions).Assembly);
		});
        
        return services;
    }
}