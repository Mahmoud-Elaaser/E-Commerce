namespace ECommerce.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order?> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetUserOrdersAsync(Guid userId);
        Task<Order> CreateOrderFromBasketAsync(Guid userId, CustomerBasket basket, Address shippingAddress, Address billingAddress);
        Task<Order> UpdateOrderStatusAsync(Guid orderId, OrderStatus status, string changedBy);
        Task<Order> CancelOrderAsync(Guid orderId, string reason);
    }
}
