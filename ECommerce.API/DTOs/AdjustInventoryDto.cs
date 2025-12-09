using ECommerce.API.Enums;

namespace ECommerce.API.DTOs
{
    public class AdjustInventoryDto
    {
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public InventoryChangeReason Reason { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
