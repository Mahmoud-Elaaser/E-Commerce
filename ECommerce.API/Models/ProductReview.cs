using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class ProductReview
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(200)]
        public string? Title { get; set; }
        [MaxLength(2000)]
        public string? ReviewText { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false;
        public int HelpfulCount { get; set; }
        public int UnhelpfulCount { get; set; }

        public Product Product { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public ICollection<ReviewVote> Votes { get; set; } = new List<ReviewVote>();
    }
}
