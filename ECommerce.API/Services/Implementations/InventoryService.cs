using ECommerce.API.Enums;
using ECommerce.API.Repositories.Interfaces;
using ECommerce.API.Services.Interfaces;

namespace ECommerce.API.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<int> CheckAvailabilityAsync(Guid productId, Guid? variantId)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            return inventory?.Quantity ?? 0;
        }

        public async Task AdjustInventoryAsync(
            Guid productId,
            Guid? variantId,
            int quantity,
            InventoryChangeReason reason,
            string notes,
            string changedBy)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            if (inventory is null)
            {
                throw new ArgumentException("Inventory record not found");
            }

            inventory.Quantity += quantity;

            if (reason == InventoryChangeReason.Restock)
            {
                inventory.LastRestocked = DateTime.UtcNow;
            }

            await _inventoryRepository.UpdateAsync(inventory);

            // Log the change
            await _inventoryRepository.LogInventoryChangeAsync(new InventoryLog
            {
                Id = Guid.NewGuid(),
                InventoryId = inventory.Id,
                ChangeAmount = quantity,
                Reason = reason,
                Notes = notes,
                ChangedBy = changedBy,
                ChangeDate = DateTime.UtcNow
            });
        }

        public async Task<IEnumerable<Inventory>> GetLowStockAsync()
        {
            return await _inventoryRepository.GetLowStockAsync();
        }
    }
}
