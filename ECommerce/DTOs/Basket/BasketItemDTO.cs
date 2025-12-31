namespace ECommerce.DTOs.Basket
{
    public record BasketItemDTO(int Id, string ProductName, string PictureUrl, decimal Price, int Quantity);
}
