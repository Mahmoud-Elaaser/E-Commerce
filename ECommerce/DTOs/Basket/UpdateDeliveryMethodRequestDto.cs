namespace ECommerce.DTOs.Basket
{
    public class UpdateDeliveryMethodRequestDto
    {
        public int DeliveryMethodId { get; set; }

        public decimal ShippingPrice { get; set; }
    }
}
