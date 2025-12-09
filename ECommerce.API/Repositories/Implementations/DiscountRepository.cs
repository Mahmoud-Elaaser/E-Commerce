using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Implementations
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _context;

        public DiscountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Discount?> GetByCodeAsync(string code)
        {
            return await _context.Discounts
                .Include(d => d.DiscountCategories)
                .Include(d => d.DiscountProducts)
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task<Discount?> GetByIdAsync(Guid id)
        {
            return await _context.Discounts
                .Include(d => d.DiscountCategories)
                .Include(d => d.DiscountProducts)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Discounts
                .Where(d => d.IsActive && d.StartDate <= now && d.EndDate >= now)
                .ToListAsync();
        }

        public async Task<Discount> CreateAsync(Discount discount)
        {
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> UpdateAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
            return discount;
        }
    }
}
