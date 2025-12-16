namespace ECommerce.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }

        public int ProductBrandId { get; set; }
        public ProductBrand ProductBrand { get; set; } = default!;

        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; } = default!;
    }
}
