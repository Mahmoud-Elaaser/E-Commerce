using AutoMapper;
using ECommerce.DTOs;
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
                  .ForMember(d => d.Total, o => o.MapFrom(s => s.SubTotal + s.DeliveryMethod.Cost))
                  .ForMember(d => d.OrderItems, o => o.MapFrom(s => s.OrderItems))
                  .ForMember(d => d.DeliveryMethodId, o => o.MapFrom(s => s.DeliveryMethodId))
                  .ForMember(d => d.OrderDate, o => o.MapFrom(s => DateTime.UtcNow))
                  .ForMember(d => d.PaymentIntentId, o => o.MapFrom(s => s.PaymentIntentId))
                  .ForMember(d => d.OrderItems, o => o.MapFrom(s => s.OrderItems))
                  .ForMember(d => d.Subtotal, o => o.MapFrom(s => s.SubTotal))
                  .ForMember(d => d.UserEmail, o => o.MapFrom(s => s.UserEmail))
                  .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                  .ForMember(d => d.ShippingAddress, o => o.MapFrom(s => s.ShippingAddress));


            CreateMap<DeliveryMethod, DeliveryMethodResult>()
                .ForMember(d => d.Cost, o => o.MapFrom(s => s.Cost));

            CreateMap<Address, AddressDto>()
                .ForMember(d => d.Street, o => o.MapFrom(s => s.Street))
                .ForMember(d => d.City, o => o.MapFrom(s => s.City))
                .ForMember(d => d.Country, o => o.MapFrom(s => s.Country))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName))
                .ReverseMap();
        }
    }
}
