using ECommerce.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class Address
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public AddressType AddressType { get; set; }
        [Required, MaxLength(200)]
        public string Street { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string State { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string Country { get; set; } = string.Empty;
        public bool IsDefault { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
