namespace ECommerce.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }

        public int ProductBrandId { get; set; }
        public string ProductBrandName { get; set; }

        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
    }
}
