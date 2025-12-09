using ECommerce.API.Services.Interfaces;

namespace ECommerce.API.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<IEnumerable<ProductReview>> GetProductReviewsAsync(Guid productId)
        {
            return await _reviewRepository.GetProductReviewsAsync(productId);
        }

        public async Task<ProductReview> CreateReviewAsync(ProductReview review)
        {
            // Verify user purchased the product
            var hasPurchased = await _reviewRepository.HasUserPurchasedProductAsync(review.UserId, review.ProductId);
            if (!hasPurchased)
            {
                throw new InvalidOperationException("Only verified purchasers can review products");
            }

            review.ReviewDate = DateTime.UtcNow;
            review.IsApproved = true; // Auto-approve for now

            return await _reviewRepository.CreateAsync(review);
        }

        public async Task<ProductReview> VoteOnReviewAsync(Guid reviewId, Guid userId, bool isHelpful)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new ArgumentException("Review not found");
            }

            // Check if user already voted
            var existingVote = review.Votes.FirstOrDefault(v => v.UserId == userId);
            if (existingVote != null)
            {
                throw new InvalidOperationException("User has already voted on this review");
            }

            if (isHelpful)
                review.HelpfulCount++;
            else
                review.UnhelpfulCount++;

            review.Votes.Add(new ReviewVote
            {
                Id = Guid.NewGuid(),
                ReviewId = reviewId,
                UserId = userId,
                IsHelpful = isHelpful
            });

            return await _reviewRepository.UpdateAsync(review);
        }
    }
}
