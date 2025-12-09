using ECommerce.API.Services.Interfaces;

namespace ECommerce.API.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize)
        {
            return await _productRepository.GetAllAsync(page, pageSize);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int page, int pageSize)
        {
            return await _productRepository.SearchAsync(searchTerm, page, pageSize);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            // Generate SKU if not provided
            if (string.IsNullOrEmpty(product.SKU))
            {
                product.SKU = await GenerateSKUAsync(product);
            }

            // Validate SKU uniqueness
            if (await _productRepository.SKUExistsAsync(product.SKU))
            {
                throw new InvalidOperationException("SKU already exists");
            }

            return await _productRepository.CreateAsync(product);
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var existing = await _productRepository.GetByIdAsync(product.Id);
            if (existing == null)
            {
                throw new ArgumentException("Product not found");
            }

            return await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            await _productRepository.DeleteAsync(id);
        }

        private async Task<string> GenerateSKUAsync(Product product)
        {
            var random = new Random();
            string sku;
            bool exists;

            do
            {
                var prefix = product.Category?.Name?.Substring(0, Math.Min(3, product.Category.Name.Length)).ToUpper() ?? "PRD";
                var suffix = random.Next(1000, 9999);
                sku = $"{prefix}-{suffix}";
                exists = await _productRepository.SKUExistsAsync(sku);
            } while (exists);

            return sku;
        }
    }


}
