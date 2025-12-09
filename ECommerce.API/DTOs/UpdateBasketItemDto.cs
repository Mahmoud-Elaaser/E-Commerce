namespace ECommerce.API.DTOs
{
    public class UpdateBasketItemDto
    {
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
