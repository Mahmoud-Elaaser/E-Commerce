namespace ECommerce.API.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<ProductReview>> GetProductReviewsAsync(Guid productId);
        Task<ProductReview?> GetByIdAsync(Guid id);
        Task<ProductReview> CreateAsync(ProductReview review);
        Task<ProductReview> UpdateAsync(ProductReview review);
        Task<bool> HasUserPurchasedProductAsync(Guid userId, Guid productId);
    }
}
