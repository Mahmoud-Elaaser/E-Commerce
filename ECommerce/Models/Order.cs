using ECommerce.Enums;

namespace ECommerce.Models
{
    public class Order
    {
        public Order()
        {

        }
        public Order(string userEmail,
            Address shippingAddress,
            ICollection<OrderItem> orderItems,
            DeliveryMethod deliveryMethod, decimal subTotal, string paymentIntentId)
        {
            UserEmail = userEmail;
            ShippingAddress = shippingAddress;
            OrderItems = orderItems;
            DeliveryMethod = deliveryMethod;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserEmail { get; set; } = string.Empty;
        public Address ShippingAddress { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public DeliveryMethod DeliveryMethod { get; set; }
        public int? DeliveryMethodId { get; set; }

        public decimal SubTotal { get; set; }
        public string PaymentIntentId { get; set; }

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();




    }
}
