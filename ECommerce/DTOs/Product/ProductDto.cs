namespace ECommerce.DTOs.Product
{
    public record ProductDto(
        int Id, string Name, string Description,
        string? ImageUrl, decimal Price,
        int ProductBrandId, string ProductBrandName,
        int ProductTypeId, string ProductTypeName
    );

}
