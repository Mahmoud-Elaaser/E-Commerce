using AutoMapper;
using ECommerce.DTOs.Order;
using ECommerce.Models;

namespace ECommerce.MappingProfile
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {


            CreateMap<OrderItem, OrderItemDto>()
                 .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductItem.ProductId))
                 .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ProductItem.ProductName))
                 .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.ProductItem.PictureUrl))
                 .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<OrderItemPictureUrlResolver>());

            CreateMap<Order, OrderResult>()
                  .ForMember(d => d.Status, o => o.MapFrom(s => s.OrderStatus))
                  .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                  .ForMember(d => d.Total, o => o.MapFrom(s => s.SubTotal + s.DeliveryMethod.Cost));

            CreateMap<DeliveryMethod, DeliveryMethodResult>()
                .ForMember(d => d.Cost, o => o.MapFrom(s => s.Cost));
        }
    }
}
