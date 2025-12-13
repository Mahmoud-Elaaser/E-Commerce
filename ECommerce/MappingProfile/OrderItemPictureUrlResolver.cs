using AutoMapper;
using ECommerce.DTOs.Order;
using ECommerce.Models;

namespace ECommerce.MappingProfile
{
    public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly IConfiguration _configuration;

        public OrderItemPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source.Product.PictureUrl))
            {
                return string.Empty;
            }

            return $"{_configuration["BaseUrl"]}{source.Product.PictureUrl}";
        }
    }
}
