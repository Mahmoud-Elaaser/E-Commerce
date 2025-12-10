namespace ECommerce.DTOs.Order
{
    public class OrderRequest
    {
        public string BasketId { get; init; }
        public AddressDto ShipToAddress { get; init; }
        public int DeliveryMethodId { get; init; }
    }
}
