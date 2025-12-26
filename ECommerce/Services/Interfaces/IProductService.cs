using ECommerce.DTOs;
using ECommerce.DTOs.Pagination;
using ECommerce.DTOs.Product;

namespace ECommerce.Services.Interfaces
{
    public interface IProductService
    {
        Task<ResponseDto> AddProductAsync(AddOrUpdateProductDto addProductDto);
        Task<ResponseDto> DeleteProductAsync(int productId);
        Task<ResponseDto> GetAllProductsAsync(ProductPaginationParams paginationParams);
        Task<ResponseDto> GetProductByIdAsync(int productId, bool includeBrand = false, bool includeType = false);
        Task<ResponseDto> GetProductsByBrandAsync(int brandId, PaginationParams paginationParams);
        Task<ResponseDto> GetProductsByTypeAsync(int typeId, PaginationParams paginationParams);
        Task<ResponseDto> UpdateProductAsync(int productId, AddOrUpdateProductDto updateProductDto);
        Task<ResponseDto> SearchProductsAsync(string searchTerm, PaginationParams paginationParams);
        //Task<ResponseDto> UpdateProductStockAsync(int productId, int quantity);

    }
}
