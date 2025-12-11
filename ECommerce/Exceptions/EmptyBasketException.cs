namespace ECommerce.Exceptions
{
    public class EmptyBasketException : Exception
    {
        public string BasketId { get; }
        public EmptyBasketException(string basketId)
            : base($"Basket '{basketId}' is empty or not found")
        {
            BasketId = basketId;
        }
    }
}
