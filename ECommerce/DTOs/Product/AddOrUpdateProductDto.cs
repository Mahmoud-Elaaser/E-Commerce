namespace ECommerce.DTOs.Product
{
    public record AddOrUpdateProductDto(
        string Name,
        string Description,
        IFormFile? ImageFile,
        decimal Price,
        int ProductBrandId,
        int ProductTypeId
    );
}
