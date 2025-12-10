using ECommerce.DTOs.Basket;

namespace ECommerce.Services.Interfaces
{
    public interface IPaymentService
    {
        public Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId);

        public Task UpdateOrderPaymentStatusAsync(string json, string header);

    }
}
