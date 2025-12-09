using ECommerce.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        [MaxLength(100)]
        public string? TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
        [MaxLength(1000)]
        public string? GatewayResponse { get; set; }

        public Order Order { get; set; } = null!;
    }
}
