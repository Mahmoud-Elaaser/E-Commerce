namespace ECommerce.DTOs.Order
{
    public class OrderResult
    {
        public Guid Id { get; init; }
        public string UserEmail { get; init; } = string.Empty;
        public AddressDto ShippingAddress { get; init; }

        public ICollection<OrderItemDto> OrderItems { get; init; } = new List<OrderItemDto>();


        public string Status { get; init; }
        public string DeliveryMethod { get; init; }
        public int? DeliveryMethodId { get; init; }

        public decimal Subtotal { get; init; }
        public string PaymentIntentId { get; init; } = string.Empty;

        public DateTimeOffset OrderDate { get; init; } = DateTimeOffset.Now;

        public decimal Total { get; init; }
    }
}
