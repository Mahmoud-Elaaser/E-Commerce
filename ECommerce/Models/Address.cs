namespace ECommerce.Models
{
    public class Address
    {
        public Address()
        {
        }
        public Address(string firstName, string lastName, string street, string city, string country)
        {
            FirstName = firstName;
            LastName = lastName;
            Street = street;
            City = city;
            Country = country;
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;
    }
}
