namespace ECommerce.Models
{
    public class BasketItem
    {
        public int Id { get; set; }

        //public int ProductId { get; set; }

        public string ProductName { get; set; } = default!;
        public string ProductType { get; set; } = default!;
        public string ProductBrand { get; set; } = default!;

        public string PictureUrl { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
