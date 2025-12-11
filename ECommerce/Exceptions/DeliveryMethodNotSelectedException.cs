namespace ECommerce.Exceptions
{
    public class DeliveryMethodNotSelectedException : Exception
    {
        public string BasketId { get; }

        public DeliveryMethodNotSelectedException(string basketId)
            : base($"Delivery method not selected for basket '{basketId}'")
        {
            BasketId = basketId;
        }
    }
}
