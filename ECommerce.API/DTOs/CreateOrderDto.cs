using ECommerce.API.Models;

namespace ECommerce.API.DTOs
{
    public class CreateOrderDto
    {
        public string BasketId { get; set; } = string.Empty;
        public Address ShippingAddress { get; set; } = null!;
        public Address BillingAddress { get; set; } = null!;
    }
}
