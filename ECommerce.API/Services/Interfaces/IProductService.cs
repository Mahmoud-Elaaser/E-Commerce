namespace ECommerce.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int page, int pageSize);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid id);
    }
}
