using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;
using FluentValidation;
using System.Transactions;
using AutoMapper;
using Market.Domain.ValueObjects; 

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
    IMapper mapper)
    : IOrdersService
{
    public async Task<IEnumerable<OrderListDto>> GetAll(long userId, CancellationToken cancellationToken)
    {
        var orders = await ordersRepository.GetAll(userId, cancellationToken);
        return mapper.Map<IEnumerable<OrderListDto>>(orders); 
    }

    public async Task<OrderDto> Get(long id, long userId, CancellationToken cancellationToken)
    {
        var order = await GetOrderOrThrowAsync(id, userId, cancellationToken); 
        return mapper.Map<OrderDto>(order);
    }

    public async Task Confirm(long id, long userId, CancellationToken cancellationToken)
    {
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
        await inventoryService.DeductReservedProductsAsync(order.Items, cancellationToken);
        
        transaction.Complete();
    }

    public async Task Deliver(long id, long userId, CancellationToken ct)
    {
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
            
            Price = x.TotalPrice.Amount, 
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


    private async Task<Order> GetOrderOrThrowAsync(long id, long userId, CancellationToken ct)
    {
        var order = await ordersRepository.Get(id, userId, ct);
        return order ?? throw new InvalidOperationException($"Замовлення з ID {id} не знайдено.");
    }
}