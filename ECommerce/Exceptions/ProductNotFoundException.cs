namespace ECommerce.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public int ProductId { get; }
        public ProductNotFoundException(int productId)
            : base($"Product with ID {productId} not found")
        {
            ProductId = productId;
        }
    }
}
