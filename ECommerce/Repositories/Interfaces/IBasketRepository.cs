using ECommerce.Models;

namespace ECommerce.Repositories.Interfaces
{
    public interface IBasketRepository
    {
        Task<Basket?> GetBasketAsync(string basketId);
        Task<Basket?> UpdateBasketAsync(Basket basket);
        Task<bool> DeleteBasketAsync(string basketId);
        Task<bool> ClearBasketAsync(string basketId);
    }
}
