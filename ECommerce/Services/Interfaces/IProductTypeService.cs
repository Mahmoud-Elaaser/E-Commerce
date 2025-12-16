using ECommerce.DTOs;
using ECommerce.DTOs.Product;

namespace ECommerce.Services.Interfaces
{
    public interface IProductTypeService
    {
        Task<ResponseDto> GetAllTypesAsync();
        Task<ResponseDto> GetTypeByIdAsync(int id);
        Task<ResponseDto> CreateTypeAsync(CreateOrUpdateTypeDto dto);
        Task<ResponseDto> UpdateTypeAsync(int id, CreateOrUpdateTypeDto dto);
        Task<ResponseDto> DeleteTypeAsync(int id);
        Task<bool> TypeExistsAsync(int id);
    }
}
