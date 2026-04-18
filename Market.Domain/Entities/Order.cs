using Market.Domain.Enums;

namespace Market.Domain.Entities;

public class Order
{
    public long Id { get; set; }
    public long UserId { get; set; }
    
    // 1. ЗМІНА: Робимо set приватним! 
    // Тепер ніхто не зможе випадково написати order.Status = OrderStatus.Delivered в обхід правил.
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
    public User? User { get; set; }
    public decimal TotalPrice { get; set; }
    public Guid? AppliedPromotionId { get; set; }
    public Promotion? AppliedPromotion { get; set; }

    // 2. ДОДАЄМО ПОВЕДІНКУ: Методи для зміни стану замовлення
    public void Confirm() => ChangeStatus(OrderStatus.Confirmed);
    
    public void Ship() => ChangeStatus(OrderStatus.Shipped);
    
    public void Deliver() => ChangeStatus(OrderStatus.Delivered);
    
    public void Cancel() => ChangeStatus(OrderStatus.Cancelled);

    // 3. ІНКАПСУЛЯЦІЯ: Переносимо сюди ту саму логіку перевірки з твого сервісу
    private void ChangeStatus(OrderStatus nextStatus)
    {
        var isValid = (Status, nextStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Confirmed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            
            (OrderStatus.Confirmed, OrderStatus.Shipped) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            
            _ => false
        };

        if (!isValid)
        {
            throw new InvalidOperationException($"Логічна помилка: неможливо змінити статус замовлення з {Status} на {nextStatus}.");
        }

        Status = nextStatus;
    }
}