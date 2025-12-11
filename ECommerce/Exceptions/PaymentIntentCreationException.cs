namespace ECommerce.Exceptions
{
    public class PaymentIntentCreationException : Exception
    {
        public string BasketId { get; }

        public PaymentIntentCreationException(string basketId, string message, Exception innerException)
            : base($"Failed to create payment intent for basket '{basketId}': {message}", innerException)
        {
            BasketId = basketId;
        }
    }

}
