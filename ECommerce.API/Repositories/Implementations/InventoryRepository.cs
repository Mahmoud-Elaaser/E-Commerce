using ECommerce.API.Repositories.Interfaces;

namespace ECommerce.API.Repositories.Implementations
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Inventory?> GetByProductIdAsync(Guid productId)
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public async Task<IEnumerable<Inventory>> GetLowStockAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.Quantity <= i.LowStockThreshold)
                .ToListAsync();
        }

        public async Task<Inventory> UpdateAsync(Inventory inventory)
        {
            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task LogInventoryChangeAsync(InventoryLog log)
        {
            _context.InventoryLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
