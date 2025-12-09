using ECommerce.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class InventoryLog
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public int ChangeAmount { get; set; }
        public InventoryChangeReason Reason { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;

        public Inventory Inventory { get; set; } = null!;
    }
}
