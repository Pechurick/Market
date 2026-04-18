using AutoMapper;
using Market.Application.Models.Responses;
using Market.Domain.Entities;

namespace Market.Application.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // 1. Мапінг для списку замовлень (GetAll, GetPaged)
        CreateMap<Order, OrderListDto>();

        // 2. Мапінг для детального замовлення (Get)
        CreateMap<Order, OrderDto>();

        // 3. Мапінг для вкладених колекцій (елементи замовлення)
        CreateMap<OrderItem, OrderItemDto>();

        // 4. Мапінг для товару всередині елемента замовлення
        CreateMap<Product, OrderItemProductDto>();
    }
}