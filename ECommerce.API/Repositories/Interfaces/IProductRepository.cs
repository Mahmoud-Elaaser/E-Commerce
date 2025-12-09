namespace ECommerce.API.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<Product>> SearchAsync(string searchTerm, int page, int pageSize);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task<bool> SKUExistsAsync(string sku);
    }
}
