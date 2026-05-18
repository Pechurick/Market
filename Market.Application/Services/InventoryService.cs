using Market.Application.Models.Requests;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;

namespace Market.Application.Services;

public class InventoryService(IProductsRepository productsRepository) : IInventoryService
{
    public async Task ReserveProductsAsync(List<Product> products, IEnumerable<OrderItemCreateDto> requestedItems, CancellationToken cancellationToken)
    {
        foreach (var item in requestedItems)
        {
            var product = products.First(p => p.Id == item.ProductId);
            
            if (product.Amount < item.Amount)
            {
                throw new InvalidOperationException($"Недостатньо товару '{product.Name}'. Запитано: {item.Amount}, в наявності: {product.Amount}");
            }
            
            product.Amount -= item.Amount;
            product.ReservedAmount += item.Amount;
        }

        await productsRepository.UpdateRange(products, cancellationToken);
    }

    public async Task DeductReservedProductsAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken)
    {
        var productIds = orderItems.Select(x => x.ProductId).ToList();
        var products = (await productsRepository.Get(productIds, cancellationToken)).ToList();
        
        foreach (var product in products)
        {
            var amountInOrder = orderItems.First(i => i.ProductId == product.Id).Quantity;
            product.ReservedAmount -= amountInOrder; 
        }

        await productsRepository.UpdateRange(products, cancellationToken);
    }

    public async Task ReleaseReservedProductsAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken)
    {
        var productIds = orderItems.Select(x => x.ProductId).ToList();
        var products = (await productsRepository.Get(productIds, cancellationToken)).ToList();
        
        foreach (var product in products)
        {
            var amountInOrder = orderItems.First(i => i.ProductId == product.Id).Quantity;
            
            product.ReservedAmount -= amountInOrder; 
            product.Amount += amountInOrder;
        }

        await productsRepository.UpdateRange(products, cancellationToken);
    }
}