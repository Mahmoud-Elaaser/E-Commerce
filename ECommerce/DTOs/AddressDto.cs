namespace ECommerce.DTOs
{
    public record AddressDto(
        string FirstName, string LastName,
        string Street, string City, string Country
    );

}
