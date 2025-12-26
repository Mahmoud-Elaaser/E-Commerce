namespace ECommerce.DTOs.Product
{
    public class AddOrUpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile? ImageFile { get; set; }

        public decimal Price { get; set; }

        public int ProductBrandId { get; set; }

        public int ProductTypeId { get; set; }

    }
}
