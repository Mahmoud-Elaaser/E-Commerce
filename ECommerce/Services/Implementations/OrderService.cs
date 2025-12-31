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

        public OrderService(
            IBasketRepository basketRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResult> CreateOrderAsync(OrderRequest request, string userEmail)
        {

            var basket = await _basketRepository.GetBasketAsync(request.BasketId);
            if (basket == null || basket.Items.Count == 0)
                throw new EmptyBasketException(request.BasketId);

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
                throw new PaymentIntentMissingException(request.BasketId);

            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>()
                    .GetByIdAsync(item.Id);

                if (product == null)
                    throw new ProductNotFoundException(item.Id);

                orderItems.Add(CreateOrderItem(item, product));
            }


            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                .GetByIdAsync(request.DeliveryMethodId);

            if (deliveryMethod == null)
                throw new DeliveryMethodNotFoundException(request.DeliveryMethodId);

            /// Handle existing orders with same payment intent
            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetAsyncWithSpec(spec);

            if (existingOrder != null)
                _unitOfWork.Repository<Order>().Delete(existingOrder);

            try
            {
                var subTotal = orderItems.Sum(item => item.Price * item.Quantity);
                var address = _mapper.Map<Address>(request.ShipToAddress);
                var order = new Order(
                    userEmail, address, orderItems, deliveryMethod, subTotal, basket.PaymentIntentId);

                await _unitOfWork.Repository<Order>().AddAsync(order);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<OrderResult>(order);
            }
            catch (Exception ex)
            {
                throw new OrderCreationException(request.BasketId, userEmail, ex);
            }
        }

        public async Task<IEnumerable<DeliveryMethodResult>> GetDeliveryMethodsAsync()
        {

            var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();

            var mappedDeliveryMethods = _mapper.Map<IEnumerable<DeliveryMethodResult>>(deliveryMethods);

            return mappedDeliveryMethods;

        }

        public async Task<OrderResult> GetOrderByIdAsync(Guid id)
        {
            var spec = new OrderWithIncludeSpecifications(id);
            var order = await _unitOfWork.Repository<Order>().GetAsyncWithSpec(spec);

            if (order == null)
                throw new OrderNotFoundException(id);

            return _mapper.Map<OrderResult>(order);
        }

        public async Task<IEnumerable<OrderResult>> GetOrdersByEmailAsync(string email)
        {

            var orders = await _unitOfWork.Repository<Order>()
                                .ListAsync(new OrderWithIncludeSpecifications(email));

            var mappedOrders = _mapper.Map<IEnumerable<OrderResult>>(orders);

            return mappedOrders;
        }

        private OrderItem CreateOrderItem(BasketItem item, Product product)
        {
            var productItemOrdered = new ProductOrderItem(product.Id, product.Name, product.ImageUrl);
            var orderItem = new OrderItem(productItemOrdered, item.Quantity, product.Price);
            return orderItem;
        }
    }
}