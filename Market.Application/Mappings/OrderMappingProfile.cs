using AutoMapper;
using Market.Application.Models.Responses;
using Market.Domain.Entities;

namespace Market.Application.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        
        CreateMap<Order, OrderListDto>()
            
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.TotalPrice.Amount));

        
        CreateMap<Order, OrderDto>()
            
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.TotalPrice.Amount));

        
        CreateMap<OrderItem, OrderItemDto>()
            
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Quantity));

        
        CreateMap<Product, OrderItemProductDto>();
    }
}