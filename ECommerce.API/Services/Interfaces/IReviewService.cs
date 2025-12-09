namespace ECommerce.API.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ProductReview>> GetProductReviewsAsync(Guid productId);
        Task<ProductReview> CreateReviewAsync(ProductReview review);
        Task<ProductReview> VoteOnReviewAsync(Guid reviewId, Guid userId, bool isHelpful);
    }
}
