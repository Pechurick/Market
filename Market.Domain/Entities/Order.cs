using Market.Domain.Enums;
using Market.Domain.Common;
using Market.Domain.Events;
using Market.Domain.ValueObjects;

namespace Market.Domain.Entities;


public class Order : AggregateRoot 
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; private set; }
    public Money TotalPrice { get; private set; }
    public Guid? AppliedPromotionId { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public User? User { get; private set; }
    public Promotion? AppliedPromotion { get; private set; }

    
    private Order() 
    { 
        TotalPrice = null!; 
    }

    public static Order Create(long userId, Guid? promotionId = null)
    {
        var order = new Order
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            AppliedPromotionId = promotionId,
            TotalPrice = Money.Create(0, "UAH")
        };

        order.RaiseDomainEvent(new OrderCreatedEvent(order.Id, userId, DateTime.UtcNow));

        return order;
    }

    
    public void AddItem(long productId, Money price, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Неможливо змінити товари, замовлення вже в обробці.");

        
        var validatedQuantity = Quantity.Create(quantity);

        var item = new OrderItem(this.Id, productId, price, validatedQuantity.Value);
        _items.Add(item);

        RecalculateTotal();
    }

    
    private void RecalculateTotal()
    {
        if (!_items.Any())
        {
            TotalPrice = Money.Create(0, "UAH");
            return;
        }

        
        var sum = _items.Sum(x => x.Price.Amount * x.Quantity.Value);
        
        
        TotalPrice = Money.Create(sum, _items.First().Price.Currency); 
    }

    
    public void Confirm() => ChangeStatus(OrderStatus.Confirmed);
    public void Ship() => ChangeStatus(OrderStatus.Shipped);
    public void Deliver() => ChangeStatus(OrderStatus.Delivered);
    public void Cancel() => ChangeStatus(OrderStatus.Cancelled);

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
            throw new InvalidOperationException($"Логічна помилка: неможливо змінити статус замовлення з {Status} на {nextStatus}.");

        Status = nextStatus;
    }
}