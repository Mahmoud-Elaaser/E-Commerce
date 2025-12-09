namespace ECommerce.API.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<int> CheckAvailabilityAsync(Guid productId, Guid? variantId);
        Task AdjustInventoryAsync(Guid productId, Guid? variantId, int quantity, InventoryChangeReason reason, string notes, string changedBy);
        Task<IEnumerable<Inventory>> GetLowStockAsync();
    }
}
