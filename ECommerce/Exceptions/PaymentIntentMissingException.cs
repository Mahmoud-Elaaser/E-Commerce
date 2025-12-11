namespace ECommerce.Exceptions
{
    public class PaymentIntentMissingException : Exception
    {
        public string BasketId { get; }

        public PaymentIntentMissingException(string basketId)
            : base($"Basket '{basketId}' has no payment intent. Payment must be initiated before creating an order.")
        {
            BasketId = basketId;
        }
    }
}
