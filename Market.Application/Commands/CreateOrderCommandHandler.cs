using MediatR;
using Market.Application.Models.Requests;
using Market.Domain.ValueObjects;
using Market.Domain.Entities;
using System.Transactions;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using FluentValidation;

namespace Market.Application.Commands;


public record CreateOrderCommand(long UserId, OrderCreateDto Request) : IRequest<long>;


public class CreateOrderCommandHandler(
    IOrdersRepository ordersRepository,
    IProductsRepository productsRepository,
    IUsersRepository usersRepository,
    IPricingEngine pricingEngine,
    IInventoryService inventoryService,
    IValidator<OrderCreateDto> createValidator) 
    : IRequestHandler<CreateOrderCommand, long>
{
    public async Task<long> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        
        await createValidator.ValidateAndThrowAsync(command.Request, cancellationToken);

        
        var user = await usersRepository.Get(command.UserId, cancellationToken)
            ?? throw new InvalidOperationException($"Користувача з ID {command.UserId} не знайдено.");
        
        if (user.IsDeleted) throw new InvalidOperationException("Акаунт користувача видалено.");

        
        var requestedIds = command.Request.Items!.Select(x => x.ProductId).Distinct().ToList();
        var products = (await productsRepository.Get(requestedIds, cancellationToken)).ToList();

        
        var (_, promoId) = await pricingEngine.CalculateFinalPriceAsync(
            command.UserId, products, command.Request.Items, command.Request.PromoCode, cancellationToken);

        
        var order = Order.Create(command.UserId, promoId);

        foreach (var itemDto in command.Request.Items)
        {
            var product = products.First(p => p.Id == itemDto.ProductId);
            var priceVo = Money.Create(product.Price, "UAH");
            order.AddItem(product.Id, priceVo, itemDto.Amount); 
        }

        
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        await ordersRepository.Add(order, cancellationToken);
        await inventoryService.ReserveProductsAsync(products, command.Request.Items, cancellationToken);
        
        transaction.Complete();

        return order.Id;
    }
}