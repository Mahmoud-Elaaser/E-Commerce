using ECommerce.DTOs.Order;

namespace ECommerce.Services.Interfaces
{
    public interface IOrderService
    {
        // Get OrderByID ==> OrderResult(Guid id))
        public Task<OrderResult> GetOrderByIdAsync(Guid id);
        // Get Orders For User By Email ==> List<OrderResult>(string email)
        public Task<IEnumerable<OrderResult>> GetOrdersByEmailAsync(string email);
        // Create Order ==> OrderResultDto(Orderresult (BasketId & ShippingAddress), userEmail)
        public Task<OrderResult> CreateOrderAsync(OrderRequest orderRequest, string userEmail);
        // Get All Delivery Methods ==> IEnumerable<DeliveryResult>()
        public Task<IEnumerable<DeliveryMethodResult>> GetDeliveryMethodsAsync();
    }
}
