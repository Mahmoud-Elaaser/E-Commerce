using ECommerce.Enums;

namespace ECommerce.Models
{
    public class Order
    {
        public Order()
        {
        }

        public Order(
            string userEmail,
            Address address,
            DeliveryMethod deliveryMethod,
            ICollection<OrderItem> items,
            decimal subTotal,
            string paymentIntentId)
        {
            UserEmail = userEmail;
            OrderAddress = address;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
            OrderStatus = OrderStatus.Pending;
            OrderDate = DateTimeOffset.UtcNow;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserEmail { get; set; } = default!;
        public Address OrderAddress { get; set; } = default!;

        public DeliveryMethod DeliveryMethod { get; set; }
        public int DeliveryMethodId { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

        public string PaymentIntentId { get; set; } = default!;

        public decimal SubTotal { get; set; }

        public decimal Total => SubTotal + DeliveryMethod.Cost;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
