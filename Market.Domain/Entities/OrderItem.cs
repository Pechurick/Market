namespace Market.Domain.Entities;
using Market.Domain.ValueObjects; 

public class OrderItem
{
    public long Id { get; private set; }
    public long OrderId { get; private set; }
    public long ProductId { get; private set; }
    
    public Money Price { get; private set; } 
    public Quantity Quantity { get; private set; }

    public Order? Order { get; private set; }
    public Product? Product { get; private set; }

    private OrderItem() 
    { 
        Price = null!; 
        Quantity = null!; 
    }

    internal OrderItem(long orderId, long productId, Money price, int quantity)
    {
        OrderId = orderId;
        ProductId = productId;
        Price = price;
        
        Quantity = Quantity.Create(quantity); 
    }
}