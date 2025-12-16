using AutoMapper;
using ECommerce.DTOs.Order;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using ECommerce.Specifications;

namespace ECommerce.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IBasketRepository basketRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<OrderService> logger)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<OrderResult> CreateOrderAsync(OrderRequest request, string userEmail)
        {
            _logger.LogInformation(
                "Creating order for user {UserEmail} with basket {BasketId}",
                userEmail, request.BasketId);

            var basket = await _basketRepository.GetBasketAsync(request.BasketId);
            if (basket == null || basket.Items.Count == 0)
            {
                _logger.LogWarning(
                    "Order creation failed: Basket {BasketId} is empty or not found",
                    request.BasketId);
                throw new EmptyBasketException(request.BasketId);
            }

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                _logger.LogWarning(
                    "Order creation failed: Basket {BasketId} has no payment intent",
                    request.BasketId);
                throw new PaymentIntentMissingException(request.BasketId);
            }

            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>()
                    .GetByIdAsync(item.Id);

                if (product == null)
                {
                    _logger.LogWarning(
                        "Order creation failed: Product {ProductId} not found for basket {BasketId}",
                        item.Id, request.BasketId);
                    throw new ProductNotFoundException(item.Id);
                }

                orderItems.Add(CreateOrderItem(item, product));
            }

            _logger.LogDebug(
                "Created {ItemCount} order items for basket {BasketId}",
                orderItems.Count, request.BasketId);

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                .GetByIdAsync(request.DeliveryMethodId);

            if (deliveryMethod == null)
            {
                _logger.LogWarning(
                    "Order creation failed: Delivery method {DeliveryMethodId} not found",
                    request.DeliveryMethodId);
                throw new DeliveryMethodNotFoundException(request.DeliveryMethodId);
            }

            /// Handle existing orders with same payment intent
            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetAsyncWithSpec(spec);

            if (existingOrder != null)
            {
                _logger.LogInformation(
                    "Deleting existing order {OrderId} for payment intent {PaymentIntentId}",
                    existingOrder.Id, basket.PaymentIntentId);
                _unitOfWork.Repository<Order>().Delete(existingOrder);
            }

            try
            {
                var subTotal = orderItems.Sum(item => item.Price * item.Quantity);
                var address = _mapper.Map<Address>(request.ShipToAddress);
                var order = new Order(
                    userEmail, address, orderItems, deliveryMethod, subTotal, basket.PaymentIntentId);

                await _unitOfWork.Repository<Order>().AddAsync(order);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation(
                    "Successfully created order {OrderId} for user {UserEmail} with total {Total:C}",
                    order.Id, userEmail, subTotal + deliveryMethod.Cost);

                return _mapper.Map<OrderResult>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to create order for user {UserEmail} with basket {BasketId}",
                    userEmail, request.BasketId);
                throw new OrderCreationException(request.BasketId, userEmail, ex);
            }
        }

        public async Task<IEnumerable<DeliveryMethodResult>> GetDeliveryMethodsAsync()
        {
            _logger.LogDebug("Retrieving all delivery methods");

            try
            {
                var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>()
                    .ListAllAsync();

                var mappedDeliveryMethods = _mapper.Map<IEnumerable<DeliveryMethodResult>>(deliveryMethods);

                _logger.LogDebug(
                    "Retrieved {Count} delivery methods",
                    mappedDeliveryMethods.Count());

                return mappedDeliveryMethods;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve delivery methods");
                throw;
            }
        }

        public async Task<OrderResult> GetOrderByIdAsync(Guid id)
        {
            _logger.LogDebug("Retrieving order {OrderId}", id);
            var spec = new OrderWithIncludeSpecifications(id);
            var order = await _unitOfWork.Repository<Order>().GetAsyncWithSpec(spec);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", id);
                throw new OrderNotFoundException(id);
            }

            _logger.LogDebug("Successfully retrieved order {OrderId}", id);

            return _mapper.Map<OrderResult>(order);
        }

        public async Task<IEnumerable<OrderResult>> GetOrdersByEmailAsync(string email)
        {
            _logger.LogDebug("Retrieving orders for user {UserEmail}", email);

            try
            {
                var orders = await _unitOfWork.Repository<Order>()
                    .ListAsync(new OrderWithIncludeSpecifications(email));

                var mappedOrders = _mapper.Map<IEnumerable<OrderResult>>(orders);

                _logger.LogDebug(
                    "Retrieved {Count} orders for user {UserEmail}",
                    mappedOrders.Count(), email);

                return mappedOrders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve orders for user {UserEmail}", email);
                throw;
            }
        }

        private OrderItem CreateOrderItem(BasketItem item, Product product)
        {
            var productItemOrdered = new ProductOrderItem(product.Id, product.Name, product.ImageUrl);
            var orderItem = new OrderItem(productItemOrdered, item.Quantity, product.Price);
            return orderItem;
        }
    }
}