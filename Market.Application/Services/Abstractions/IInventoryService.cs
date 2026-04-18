using Market.Application.Models.Requests;
using Market.Domain.Entities;

namespace Market.Application.Services.Abstractions;

public interface IInventoryService
{
    // Заморожує товари при створенні замовлення (перевіряє наявність і змінює Amount/ReservedAmount)
    Task ReserveProductsAsync(List<Product> products, IEnumerable<OrderItemCreateDto> requestedItems, CancellationToken cancellationToken);
    
    // Остаточно списує товари зі складу при відправці (Ship)
    Task DeductReservedProductsAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken);
    
    // Повертає товари на склад при скасуванні замовлення (Cancel)
    Task ReleaseReservedProductsAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken);
}