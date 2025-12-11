namespace ECommerce.Exceptions
{
    public class PaymentIntentUpdateException : Exception
    {
        public string PaymentIntentId { get; }

        public PaymentIntentUpdateException(string paymentIntentId, string message, Exception innerException)
            : base($"Failed to update payment intent '{paymentIntentId}': {message}", innerException)
        {
            PaymentIntentId = paymentIntentId;
        }
    }
}
