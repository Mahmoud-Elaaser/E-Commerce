using Microsoft.AspNetCore.Identity;

namespace ECommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public Address Address { get; set; } = new Address();

    }
}
