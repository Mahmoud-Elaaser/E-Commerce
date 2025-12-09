namespace ECommerce.API.DTOs
{
    public class CreateReviewDto
    {
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string? ReviewText { get; set; }
    }
}
