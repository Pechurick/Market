using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;
using FluentValidation;
using System.Transactions;
using AutoMapper;

namespace Market.Application.Services;

public class OrdersService(
    IOrdersRepository ordersRepository,
    IProductsRepository productsRepository,
    IUsersRepository usersRepository,
    IPricingEngine pricingEngine, 
    ILoyaltyService loyaltyService,
    IInventoryService inventoryService, 
    IValidator<OrderCreateDto> createValidator,
    IValidator<PagedRequestDto> pagedValidator,
    TimeProvider timeProvider,
    IMapper mapper)
    : IOrdersService
{
    public async Task<IEnumerable<OrderListDto>> GetAll(long userId, CancellationToken cancellationToken)
    {
        var orders = await ordersRepository.GetAll(userId, cancellationToken);
        
        // 🪄 Магія AutoMapper
        return mapper.Map<IEnumerable<OrderListDto>>(orders); 
    }

    public async Task<OrderDto> Get(long id, long userId, CancellationToken cancellationToken)
    {
        var order = await GetOrderOrThrowAsync(id, userId, cancellationToken); 

        // 🪄 Магія AutoMapper
        return mapper.Map<OrderDto>(order);
    }

    public async Task Create(long userId, OrderCreateDto request, CancellationToken cancellationToken)
    {
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await usersRepository.Get(userId, cancellationToken)
            ?? throw new InvalidOperationException($"Користувача з ID {userId} не знайдено.");
        
        if (user.IsDeleted) throw new InvalidOperationException("Акаунт користувача видалено.");
        
        var requestedIds = request.Items!.Select(x => x.ProductId).Distinct().ToList();
        var products = (await productsRepository.Get(requestedIds, cancellationToken)).ToList();

        // 🧮 Викликаємо ціноутворення
        var (finalPrice, promoId) = await pricingEngine.CalculateFinalPriceAsync(userId, products, request.Items, request.PromoCode, cancellationToken);

        var order = new Order
        {
            UserId = userId,
            Price = finalPrice, 
            AppliedPromotionId = promoId, 
            CreatedAt = timeProvider.GetLocalNow().DateTime,
            Items = products.Select(p => new OrderItem
            {
                ProductId = p.Id,
                Amount = request.Items.First(i => i.ProductId == p.Id).Amount,
                Price = p.Price
            }).ToList()
        };

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        await ordersRepository.Add(order, cancellationToken);
        
        // 📦 Делегуємо резервацію складу (цей метод сам перевірить Amount і збереже в БД)
        await inventoryService.ReserveProductsAsync(products, request.Items, cancellationToken);
        
        transaction.Complete();
    }

    public async Task Confirm(long id, long userId, CancellationToken cancellationToken)
    {
        // 👈 Використовуємо хелпер
        var order = await GetOrderOrThrowAsync(id, userId, cancellationToken);

        order.Confirm();
        await ordersRepository.Update(order, cancellationToken);
    }

    public async Task Ship(long id, long userId, CancellationToken cancellationToken)
    {
        var order = await GetOrderOrThrowAsync(id, userId, cancellationToken);
        order.Ship();

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        await ordersRepository.Update(order, cancellationToken);
        
        // 📦 Делегуємо списання складу
        await inventoryService.DeductReservedProductsAsync(order.Items, cancellationToken);
        
        transaction.Complete();
    }

    public async Task Deliver(long id, long userId, CancellationToken ct)
    {
        // 👈 Використовуємо хелпер (навіть параметр ct передається чисто)
        var order = await GetOrderOrThrowAsync(id, userId, ct);

        order.Deliver();
        await ordersRepository.Update(order, ct);

        await loyaltyService.UpdateUserLoyaltyAsync(userId, ct); 
    }

    public async Task Cancel(long id, long userId, CancellationToken cancellationToken)
    {
        var order = await GetOrderOrThrowAsync(id, userId, cancellationToken);
        order.Cancel(); 

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        await ordersRepository.Update(order, cancellationToken);
        
        // 📦 Делегуємо повернення складу
        await inventoryService.ReleaseReservedProductsAsync(order.Items, cancellationToken);
        
        transaction.Complete();
    }

    public async Task<PagedResultDto<OrderListDto>> GetPaged(
        long userId, 
        PagedRequestDto pagedRequest, 
        CancellationToken cancellationToken)
    {
        await pagedValidator.ValidateAndThrowAsync(pagedRequest, cancellationToken);
        
        var (orders, totalCount) = await ordersRepository.GetPaged(
            userId, 
            pagedRequest.PageNumber, 
            pagedRequest.PageSize, 
            cancellationToken);

        var items = orders.Select(x => new OrderListDto
        {
            Id = x.Id,
            Price = x.Price,
            CreatedAt = x.CreatedAt,
            Status = x.Status
        });

        return new PagedResultDto<OrderListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pagedRequest.PageNumber,
            PageSize = pagedRequest.PageSize
        };
    }

    // 🛠️ НАШ НОВИЙ ПРИВАТНИЙ ХЕЛПЕР
    private async Task<Order> GetOrderOrThrowAsync(long id, long userId, CancellationToken ct)
    {
        var order = await ordersRepository.Get(id, userId, ct);
        return order ?? throw new InvalidOperationException($"Замовлення з ID {id} не знайдено.");
    }
}