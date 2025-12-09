namespace ECommerce.API.Models
{
    public class ReviewVote
    {
        public Guid Id { get; set; }
        public Guid ReviewId { get; set; }
        public Guid UserId { get; set; }
        public bool IsHelpful { get; set; }

        public ProductReview Review { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
