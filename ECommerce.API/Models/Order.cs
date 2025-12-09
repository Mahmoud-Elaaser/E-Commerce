using ECommerce.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [Required, MaxLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        [MaxLength(1000)]
        public string? Notes { get; set; }

        // Embedded Address as JSON
        public string ShippingAddressJson { get; set; } = string.Empty;
        public string BillingAddressJson { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
        public Payment? Payment { get; set; }
        public Shipment? Shipment { get; set; }
    }
}
