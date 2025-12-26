namespace ECommerce.Models
{
    public class Basket
    {
        public Basket(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }

        public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
    }
}
