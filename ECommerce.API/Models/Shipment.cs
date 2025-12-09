using ECommerce.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class Shipment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ShippingMethodId { get; set; }
        [MaxLength(100)]
        public string? TrackingNumber { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? ActualDelivery { get; set; }
        [MaxLength(100)]
        public string? Carrier { get; set; }
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Processing;

        public Order Order { get; set; } = null!;
        public ShippingMethod ShippingMethod { get; set; } = null!;
    }
}
