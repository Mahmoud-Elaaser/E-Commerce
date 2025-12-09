namespace ECommerce.API.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<Discount?> GetByCodeAsync(string code);
        Task<Discount?> GetByIdAsync(Guid id);
        Task<IEnumerable<Discount>> GetActiveDiscountsAsync();
        Task<Discount> CreateAsync(Discount discount);
        Task<Discount> UpdateAsync(Discount discount);
    }
}
