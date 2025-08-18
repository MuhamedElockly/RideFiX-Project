using AutoMapper;
using Domain.Entities.e_Commerce;
using SharedData.DTOs.E_CommerceDTOs;

namespace Service.AutoMapperProfile.E_CommerceMapping
{
    public class OrderMapConfig : Profile
    {
        public OrderMapConfig()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.location))
                .ForMember(dest => dest.OrderState, opt => opt.MapFrom(src => src.orderState))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.totalPrice))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.orderItems))
                .ReverseMap();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.totalPrice))
                .ReverseMap();
        }
    }
}
