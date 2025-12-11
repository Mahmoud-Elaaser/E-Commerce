namespace ECommerce.Exceptions
{
    public class InvalidPaymentWebhookException : Exception
    {
        public InvalidPaymentWebhookException(string message)
            : base($"Invalid payment webhook: {message}")
        {
        }

        public InvalidPaymentWebhookException(string message, Exception innerException)
            : base($"Invalid payment webhook: {message}", innerException)
        {
        }
    }
}
