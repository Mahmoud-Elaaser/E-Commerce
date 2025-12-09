using ECommerce.API.Repositories.Interfaces;

namespace ECommerce.API.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductReview>> GetProductReviewsAsync(Guid productId)
        {
            return await _context.ProductReviews
                .Include(r => r.User)
                .Include(r => r.Votes)
                .Where(r => r.ProductId == productId && r.IsApproved)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<ProductReview?> GetByIdAsync(Guid id)
        {
            return await _context.ProductReviews
                .Include(r => r.User)
                .Include(r => r.Votes)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<ProductReview> CreateAsync(ProductReview review)
        {
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<ProductReview> UpdateAsync(ProductReview review)
        {
            _context.ProductReviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> HasUserPurchasedProductAsync(Guid userId, Guid productId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Delivered)
                .AnyAsync(o => o.Items.Any(i => i.ProductId == productId));
        }
    }
}
