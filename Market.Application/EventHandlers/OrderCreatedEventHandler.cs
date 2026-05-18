using MediatR;
using Microsoft.Extensions.Logging;
using Market.Domain.Events; 

namespace Market.Application.EventHandlers;


public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger) 
    : INotificationHandler<OrderCreatedEvent>
{
    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        
        logger.LogInformation($"[SIDE EFFECT] Замовлення {notification.OrderId} успішно створено для користувача {notification.UserId}. Імітуємо відправку Email...");
        
        return Task.CompletedTask;
    }
}