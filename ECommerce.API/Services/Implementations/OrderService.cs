using ECommerce.API.Enums;
using ECommerce.API.Models;
using ECommerce.API.Models.Basket;
using ECommerce.API.Services.Interfaces;
using System.Text.Json;

namespace ECommerce.API.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IInventoryService _inventoryService;

        public OrderService(IOrderRepository orderRepository, IInventoryService inventoryService)
        {
            _orderRepository = orderRepository;
            _inventoryService = inventoryService;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(Guid userId)
        {
            return await _orderRepository.GetUserOrdersAsync(userId);
        }

        public async Task<Order> CreateOrderFromBasketAsync(
            Guid userId,
            CustomerBasket basket,
            Address shippingAddress,
            Address billingAddress)
        {
            // Validate inventory for all items
            foreach (var item in basket.Items)
            {
                var available = await _inventoryService.CheckAvailabilityAsync(item.ProductId, item.ProductVariantId);
                if (available < item.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for {item.ProductName}");
                }
            }

            // Calculate totals
            var subtotal = basket.GetSubtotal();
            var taxAmount = CalculateTax(subtotal, shippingAddress.State);
            var shippingCost = CalculateShippingCost(basket);
            var discountAmount = basket.DiscountAmount;
            var total = subtotal + taxAmount + shippingCost - discountAmount;

            // Create order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderNumber = await _orderRepository.GenerateOrderNumberAsync(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Subtotal = subtotal,
                TaxAmount = taxAmount,
                ShippingCost = shippingCost,
                DiscountAmount = discountAmount,
                Total = total,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentStatus = PaymentStatus.Pending,
                ShippingAddressJson = JsonSerializer.Serialize(shippingAddress),
                BillingAddressJson = JsonSerializer.Serialize(billingAddress)
            };

            // Create order items
            foreach (var item in basket.Items)
            {
                order.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    ProductVariantId = item.ProductVariantId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    DiscountAmount = 0,
                    TotalPrice = item.Price * item.Quantity
                });
            }

            // Add initial status history
            order.StatusHistory.Add(new OrderStatusHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = OrderStatus.Pending,
                StatusDate = DateTime.UtcNow,
                ChangedBy = "System"
            });

            var createdOrder = await _orderRepository.CreateAsync(order);

            // Reserve inventory
            foreach (var item in basket.Items)
            {
                await _inventoryService.AdjustInventoryAsync(
                    item.ProductId,
                    item.ProductVariantId,
                    -item.Quantity,
                    InventoryChangeReason.Purchase,
                    $"Order {order.OrderNumber}",
                    userId.ToString()
                );
            }

            return createdOrder;
        }

        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, OrderStatus status, string changedBy)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            // Validate status transition
            ValidateStatusTransition(order.Status, status);

            order.Status = status;
            order.StatusHistory.Add(new OrderStatusHistory
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Status = status,
                StatusDate = DateTime.UtcNow,
                ChangedBy = changedBy
            });

            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<Order> CancelOrderAsync(Guid orderId, string reason)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
            {
                throw new InvalidOperationException("Cannot cancel shipped or delivered orders");
            }

            order.Status = OrderStatus.Cancelled;
            order.StatusHistory.Add(new OrderStatusHistory
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Status = OrderStatus.Cancelled,
                StatusDate = DateTime.UtcNow,
                Notes = reason,
                ChangedBy = order.UserId.ToString()
            });

            // Restore inventory
            foreach (var item in order.Items)
            {
                await _inventoryService.AdjustInventoryAsync(
                    item.ProductId,
                    item.ProductVariantId,
                    item.Quantity,
                    InventoryChangeReason.Return,
                    $"Order {order.OrderNumber} cancelled",
                    "System"
                );
            }

            return await _orderRepository.UpdateAsync(order);
        }

        private decimal CalculateTax(decimal subtotal, string state)
        {
            // Simplified tax calculation - in production, use a tax API
            var taxRate = 0.08m; // 8% default
            return subtotal * taxRate;
        }

        private decimal CalculateShippingCost(CustomerBasket basket)
        {
            // Simplified shipping calculation
            var itemCount = basket.Items.Sum(i => i.Quantity);
            return itemCount switch
            {
                <= 3 => 5.99m,
                <= 10 => 9.99m,
                _ => 14.99m
            };
        }

        private void ValidateStatusTransition(OrderStatus current, OrderStatus next)
        {
            var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
        {
            { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.Cancelled } },
            { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } },
            { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered } },
            { OrderStatus.Delivered, new List<OrderStatus> { OrderStatus.Returned } }
        };

            if (!validTransitions.ContainsKey(current) || !validTransitions[current].Contains(next))
            {
                throw new InvalidOperationException($"Cannot transition from {current} to {next}");
            }
        }
    }
}
