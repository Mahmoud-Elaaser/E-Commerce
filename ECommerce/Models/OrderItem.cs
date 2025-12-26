namespace ECommerce.Models
{
    public class OrderItem
    {
        public OrderItem()
        {

        }
        public OrderItem(ProductOrderItem product, int quantity, decimal price)
        {
            ProductItem = product;
            Quantity = quantity;
            Price = price;
        }
        public int Id { get; set; }
        public ProductOrderItem ProductItem { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
