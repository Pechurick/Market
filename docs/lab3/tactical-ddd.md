# Практична робота №4: Тактичний Domain-Driven Design

**Тема:** Перехід до багатої доменної моделі (Rich Domain Model) та реалізація Use Case створення замовлення за допомогою тактичних патернів DDD.

---

## 1. Реалізація Value Objects (Об'єкти-значення)

Для забезпечення валідації на рівні типів та імутабельності (незмінності) даних було впроваджено два ключові об'єкти-значення. Вони інкапсулюють бізнес-правила та гарантують, що доменна модель не перейде в некоректний стан.

- **Money**: Інкапсулює суму та валюту. Використовує принцип _Fail-Fast_ (викидає виняток при спробі створення від'ємної суми).
- **Address**: Інкапсулює дані для доставки, перевіряючи обов'язковість полів (місто, вулиця, індекс).

### Приклад реалізації (Money.cs та Address.cs):

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

// 2. Value Object для адреси доставки
public record Address
{
    public string City { get; init; }
    public string Street { get; init; }
    public string ZipCode { get; init; }

    public static Address Create(string city, string street, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Повна адреса є обов'язковою");

        return new Address(city, street, zipCode);
    }
}
```

### Налаштування в Infrastructure (EF Core):

Для збереження Value Objects у базі даних без створення зайвих таблиць використано механізм **Owned Entity Types**:

```csharp
// Налаштування для Money
builder.OwnsOne(o => o.TotalPrice, priceBuilder =>
{
    priceBuilder.Property(p => p.Amount).HasColumnName("Price").HasColumnType("decimal(18,2)");
    priceBuilder.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
});

// Налаштування для Address
builder.OwnsOne(o => o.DeliveryAddress, addressBuilder =>
{
    addressBuilder.Property(a => a.City).HasColumnName("DeliveryCity").IsRequired();
    addressBuilder.Property(a => a.Street).HasColumnName("DeliveryStreet").IsRequired();
    addressBuilder.Property(a => a.ZipCode).HasColumnName("DeliveryZipCode");
});
```

---

## 2. Перетворення Order на Aggregate Root

Сутність **Order** була рефакторена з "анемічної" структури у повноцінний корінь агрегату. Це забезпечує повну інкапсуляцію та захист внутрішніх інваріантів.

- **Інкапсуляція:** Усі властивості мають `private set`.
- **Захист стану:** Додавання товарів можливе лише через бізнес-метод `AddItem()`, який контролює кількість та перераховує загальну вартість.
- **Колекції:** Внутрішній список `_items` прихований, доступ назовні надається через `IReadOnlyCollection`.

```csharp
public class Order : AggregateRoot
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public Money TotalPrice { get; private set; }
    public Address DeliveryAddress { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public void AddItem(long productId, Money price, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Кількість має бути > 0");

        var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null) { /* логіка оновлення кількості */ }
        else {
            _items.Add(new OrderItem(this.Id, productId, price, quantity));
        }

        RecalculateTotal();
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
public class CreateOrderCommandHandler(...) : IRequestHandler<CreateOrderCommand, long>
{
    public async Task<long> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var products = await _productsRepository.Get(command.Request.ProductIds, ct);
        var order = Order.Create(command.UserId);

        foreach (var p in products)
            order.AddItem(p.Id, Money.Create(p.Price), 1);

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

## Критерії приймання (Definition of Done)

- [x] Сутність `Order` не має публічних сеттерів.
- [x] Використовуються Value Objects `Money` та `Address`.
- [x] При створенні замовлення через API успішно збуджується та обробляється `OrderCreatedEvent`.
- [x] Бізнес-логіка перевірки інваріантів знаходиться всередині Агрегату.
- [x] Реалізовано Command Handler для сценарію створення замовлення.
