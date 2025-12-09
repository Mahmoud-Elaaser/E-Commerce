using ECommerce.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class OrderStatusHistory
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime StatusDate { get; set; } = DateTime.UtcNow;
        [MaxLength(500)]
        public string? Notes { get; set; }
        public string ChangedBy { get; set; } = string.Empty;

        public Order Order { get; set; } = null!;
    }
}
