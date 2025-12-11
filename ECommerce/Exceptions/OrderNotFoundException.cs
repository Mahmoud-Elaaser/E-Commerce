namespace ECommerce.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public Guid OrderId { get; }

        public OrderNotFoundException(Guid orderId)
            : base($"Order with ID '{orderId}' not found")
        {
            OrderId = orderId;
        }

        public OrderNotFoundException(string paymentIntentId)
            : base($"Order with payment intent ID '{paymentIntentId}' not found")
        {
        }
    }
}
