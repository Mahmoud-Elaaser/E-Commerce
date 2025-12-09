namespace ECommerce.API.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<Inventory?> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<Inventory>> GetLowStockAsync();
        Task<Inventory> UpdateAsync(Inventory inventory);
        Task LogInventoryChangeAsync(InventoryLog log);
    }
}
