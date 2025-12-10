using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Basket
{
    public class BasketItemDTO
    {
        public int Id { get; init; }
        public string ProductName { get; init; } = null!;
        public string PictureUrl { get; init; } = null!;
        [Range(1, double.MaxValue, ErrorMessage = "Price Must Be Greater Than Zero")]
        public decimal Price { get; init; }
        [Range(1, 99, ErrorMessage = "Quantity Must Be At Least One Item")]
        public int Quantity { get; init; }
    }
}
