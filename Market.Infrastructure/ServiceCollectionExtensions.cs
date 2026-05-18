using Market.Application.Repositories;
using Market.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Market.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IBrandsRepository, BrandsRepository>();
        services.AddScoped<IPromotionsRepository, PromotionsRepository>();
        
        services.AddDbContextPool<MarketDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("Market"),
                npgsqlOptions => npgsqlOptions
                    .MigrationsAssembly(typeof(MarketDbContext).Assembly.FullName)
                    
            );
        });
        
        return services;
    }
}
