using ECommerce.Enums;

namespace ECommerce.Models
{
    public class Order : BaseEntity<Guid>
    {
        public Order()
        {

        }
        public Order(string userEmail,
            Address shippingAddress,
            ICollection<OrderItem> orderItems,
            DeliveryMethod deliveryMethod, decimal subtotal, string paymentIntentId)
        {
            UserEmail = userEmail;
            ShippingAddress = shippingAddress;
            OrderItems = orderItems;
            DeliveryMethod = deliveryMethod;
            Subtotal = subtotal;
            PaymentIntentId = paymentIntentId;
        }

        public string UserEmail { get; set; } = string.Empty;
        public Address ShippingAddress { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }


        public OrderStatus PaymentStatus { get; set; } = OrderStatus.Pending;
        public DeliveryMethod DeliveryMethod { get; set; }
        public int? DeliveryMethodId { get; set; }

        // OrderItem.Price * OrderItem.Quantity
        // Total == Subtotal + DeliveryMethod.Price [ Derieved Atribute ] --> DTO OR Mapping Profile
        public decimal Subtotal { get; set; }
        public string PaymentIntentId { get; set; }

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;




    }
}
