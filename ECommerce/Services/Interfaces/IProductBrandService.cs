using ECommerce.DTOs;
using ECommerce.DTOs.Product;

namespace ECommerce.Services.Interfaces
{
    public interface IProductBrandService
    {
        Task<ResponseDto> GetAllBrandsAsync();
        Task<ResponseDto> GetBrandByIdAsync(int id);
        Task<ResponseDto> CreateBrandAsync(CreateOrUpdateBrandDto dto);
        Task<ResponseDto> UpdateBrandAsync(int id, CreateOrUpdateBrandDto dto);
        Task<ResponseDto> DeleteBrandAsync(int id);
        Task<bool> BrandExistsAsync(int id);
    }
}
