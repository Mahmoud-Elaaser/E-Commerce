namespace ECommerce.Models
{
    public class ProductType
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
