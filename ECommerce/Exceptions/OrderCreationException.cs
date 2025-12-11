namespace ECommerce.Exceptions
{
    public class OrderCreationException : Exception
    {
        public string BasketId { get; }
        public string UserEmail { get; }

        public OrderCreationException(string basketId, string userEmail, Exception innerException)
            : base($"Failed to create order for user '{userEmail}' with basket '{basketId}'", innerException)
        {
            BasketId = basketId;
            UserEmail = userEmail;
        }
    }
}
