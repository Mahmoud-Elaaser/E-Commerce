namespace ECommerce.Exceptions
{
    public class DeliveryMethodNotFoundException : Exception
    {
        public int DeliveryMethodId { get; }

        public DeliveryMethodNotFoundException(int deliveryMethodId)
            : base($"Delivery method with ID {deliveryMethodId} not found")
        {
            DeliveryMethodId = deliveryMethodId;
        }

        public DeliveryMethodNotFoundException(int deliveryMethodId, string message)
            : base(message)
        {
            DeliveryMethodId = deliveryMethodId;
        }

        public DeliveryMethodNotFoundException(int deliveryMethodId, string message, Exception innerException)
            : base(message, innerException)
        {
            DeliveryMethodId = deliveryMethodId;
        }
    }
}