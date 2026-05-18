# Практична робота №4: Тактичний Domain-Driven Design

**Тема:** Перехід до багатої доменної моделі (Rich Domain Model) та реалізація Use Case створення замовлення за допомогою тактичних патернів DDD.

---

## 1. Реалізація Value Objects (Об'єкти-значення)

Для забезпечення валідації на рівні типів та імутабельності (незмінності) даних було впроваджено два ключові об'єкти-значення. Вони інкапсулюють бізнес-правила та гарантують, що доменна модель не перейде в некоректний стан.

- **Money**: Інкапсулює суму та валюту. Використовує принцип _Fail-Fast_ (викидає виняток при спробі створення від'ємної суми).
- **Quantity**: Інкапсулює кількість товару в замовленні. Гарантує, що кількість завжди більша за нуль і не перевищує встановлені бізнес-ліміти (наприклад, не більше 1000 одиниць).

### Приклад реалізації (Money.cs та Quantity.cs):

```csharp
// 1. Value Object для грошей
public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public static Money Create(decimal amount, string currency = "UAH")
    {
        if (amount < 0)
            throw new ArgumentException("Сума замовлення не може бути від'ємною");
        return new Money(amount, currency);
    }
}

// 2. Value Object для кількості
public record Quantity
{
    public int Value { get; init; }

    private Quantity(int value) => Value = value;

    public static Quantity Create(int value)
    {
        if (value <= 0) throw new ArgumentException("Кількість має бути > 0.");
        if (value > 1000) throw new ArgumentException("Перевищено ліміт кількості.");
        return new Quantity(value);
    }

    public static implicit operator int(Quantity quantity) => quantity.Value;
}
```

### Налаштування в Infrastructure (EF Core):

Для інтеграції Value Objects з реляційною базою даних застосовано різні підходи EF Core залежно від структури об'єкта:

```csharp
// Налаштування для Money (Owned Entity Type)
builder.OwnsOne(o => o.TotalPrice, priceBuilder =>
{
    priceBuilder.Property(p => p.Amount).HasColumnName("Price").HasColumnType("decimal(18,2)");
    priceBuilder.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
});

// Налаштування для Quantity (Value Conversion)
builder.Property(oi => oi.Quantity)
    .HasConversion(
        q => q.Value,                    // Як записувати в БД (зберігаємо звичайний int)
        v => Quantity.Create(v))         // Як читати з БД (перетворюємо int назад у Quantity)
    .HasColumnName("Quantity")
    .IsRequired();
```

---

## 2. Перетворення Order на Aggregate Root

Сутність **Order** була рефакторена з "анемічної" структури у повноцінний корінь агрегату. Це забезпечує повну інкапсуляцію та захист внутрішніх інваріантів.

- **Інкапсуляція:** Усі властивості мають `private set`.
- **Захист стану:** Додавання товарів можливе лише через бізнес-метод `AddItem()`, який застосовує VO `Quantity` та `Money`.
- **Колекції:** Внутрішній список `_items` прихований, доступ назовні надається через `IReadOnlyCollection`.

```csharp
public class Order : AggregateRoot
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public Money TotalPrice { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public void AddItem(long productId, Money price, int quantityValue)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Неможливо змінити товари, замовлення вже в обробці.");

        // Делегуємо валідацію об'єкту-значенню
        var quantity = Quantity.Create(quantityValue);

        var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null) { /* логіка оновлення кількості */ }
        else {
            _items.Add(new OrderItem(this.Id, productId, price, quantity.Value));
        }

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
}
```

---

## 3. Налаштування Domain Events Dispatcher

Для реалізації побічних ефектів (Side Effects) без порушення транзакційної цілісності агрегату було впроваджено механізм доменних подій.

1.  **Збудження події:** У методі `Order.Create()` генерується подія `OrderCreatedEvent`.
2.  **Диспетчеризація:** У `MarketDbContext` перевизначено метод `SaveChangesAsync`. Перед комітом у БД система за допомогою **MediatR** витягує події з агрегатів та публікує їх.

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    var domainEntities = ChangeTracker.Entries<AggregateRoot>()
        .Where(x => x.Entity.DomainEvents.Any()).ToList();

    var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();

    // Очищення подій перед публікацією
    domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

    foreach (var domainEvent in domainEvents)
        await _publisher.Publish(domainEvent, ct);

    return await base.SaveChangesAsync(ct);
}
```

---

## 4. Реалізація Use Case (Command & Handlers)

Сценарій створення замовлення реалізовано за допомогою патерну **CQRS**. Логіка оркестрації винесена в `CreateOrderCommandHandler`, а побічні дії — в `OrderCreatedEventHandler`.

### Command Handler (Оркестрація):

```csharp
public class CreateOrderCommandHandler(
    IOrdersRepository ordersRepository,
    IProductsRepository productsRepository)
    : IRequestHandler<CreateOrderCommand, long>
{
    public async Task<long> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var requestedIds = command.Request.Items!.Select(x => x.ProductId).Distinct().ToList();
        var products = (await productsRepository.Get(requestedIds, ct)).ToList();

        var order = Order.Create(command.UserId);

        foreach (var itemDto in command.Request.Items)
        {
            var product = products.First(p => p.Id == itemDto.ProductId);
            var priceVo = Money.Create(product.Price, "UAH");

            // Агрегат захищає інваріанти через метод AddItem
            order.AddItem(product.Id, priceVo, itemDto.Amount);
        }

        await _ordersRepository.Add(order, ct);
        return order.Id;
    }
}
```

### Event Handler (Side Effect):

```csharp
public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    : INotificationHandler<OrderCreatedEvent>
{
    public Task Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        // Імітація відправки повідомлення або логування
        logger.LogInformation($"[DOMAIN EVENT] Замовлення {notification.OrderId} успішно створено.");
        return Task.CompletedTask;
    }
}
```

---
