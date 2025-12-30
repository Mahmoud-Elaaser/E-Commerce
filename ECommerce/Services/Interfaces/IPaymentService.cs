using ECommerce.DTOs.Basket;

namespace ECommerce.Services.Interfaces
{
    public interface IPaymentService
    {
        public Task<CustomerBasketDto> CreateOrUpdatePaymentIntentAsync(string basketId);

        Task UpdateOrderPaymentStatusAsync(string json, string header);
    }
}