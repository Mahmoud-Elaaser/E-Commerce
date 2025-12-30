namespace ECommerce.Exceptions
{
    public class BasketNotFoundException : Exception
    {
        public BasketNotFoundException(string id) : base($"Basket with Id : {id}  Not Found")
        {
        }
    }
}
