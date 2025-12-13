using AutoMapper;
using ECommerce.DTOs.Basket;
using ECommerce.Models;

namespace ECommerce.MappingProfile
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<Basket, BasketDTO>().ReverseMap();
            CreateMap<BasketItem, BasketItemDTO>().ReverseMap();
        }
    }
}
