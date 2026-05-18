using Market.Application.Models.Requests;
using Market.Domain.Entities;

namespace Market.Application.Services.Abstractions;

public interface IInventoryService
{

    Task ReserveProductsAsync(List<Product> products, IEnumerable<OrderItemCreateDto> requestedItems, CancellationToken cancellationToken);
    
    
    Task DeductReservedProductsAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken);
    
    
    Task ReleaseReservedProductsAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken);
}