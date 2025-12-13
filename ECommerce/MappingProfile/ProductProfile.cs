using AutoMapper;
using ECommerce.DTOs.Product;
using ECommerce.Models;

namespace ECommerce.MappingProfile
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<AddOrUpdateProductDto, Product>();
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductBrandName,
                    opt => opt.MapFrom(src => src.ProductBrand.Name))
                .ForMember(dest => dest.ProductTypeName,
                    opt => opt.MapFrom(src => src.ProductType.Name));

            CreateMap<ProductBrand, ProductBrandDto>();

            CreateMap<ProductType, ProductTypeDto>();
        }
    }
}
