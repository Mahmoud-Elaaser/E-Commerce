using AutoMapper;
using ECommerce.DTOs.Product;
using ECommerce.Models;

namespace ECommerce.MappingProfile
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<AddOrUpdateProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductBrandName, opt => opt.MapFrom(src => src.ProductBrand.Name))
                .ForMember(dest => dest.ProductTypeName, opt => opt.MapFrom(src => src.ProductType.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<ProductBrand, ProductBrandDto>();

            CreateMap<ProductType, ProductTypeDto>();

            CreateMap<CreateOrUpdateTypeDto, ProductType>();
            CreateMap<ProductType, ProductTypeDto>();

            CreateMap<CreateOrUpdateBrandDto, ProductBrand>();
            CreateMap<ProductBrand, ProductBrandDto>();
        }
    }
}
