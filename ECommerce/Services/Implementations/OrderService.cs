using AutoMapper;
using ECommerce.DTOs.Order;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using ECommerce.Specifications;

namespace ECommerce.Services.Implementations
{
    public class OrderService(IMapper mapper,
    IBasketRepository basketRepository, IUnitOfWork unitOfWork) : IOrderService

    {
        #region Part 3 Refactor Create Order & OrderWithPaymentSpecifications
        public async Task<OrderResult> CreateOrderAsync(OrderRequest request, string userEmail)
        {

            // 1 . Shpping Address
            var address = mapper.Map<Address>(request.ShipToAddress);


            // 2. Order Items => BaskedId  --> BasketItems --> OrderItems
            var basket = await basketRepository.GetBasketAsync(request.BasketId) ?? throw new KeyNotFoundException(request.BasketId);


            // Get selected Items at Basket from Product Repository
            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.GetRepository<Models.Product, int>()
                     .GetAsync(item.Id) ?? throw new KeyNotFoundException(item.Id.ToString());
                orderItems.Add(CreateOrderItem(item, product));

            }

            // 3. Delivery Method  
            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetAsync(request.DeliveryMethodId) ?? throw new KeyNotFoundException(request.DeliveryMethodId.ToString());

            // 4. Subtotal --> item.Price * item.Quantity
            var orderrepo = unitOfWork.GetRepository<Models.Order, Guid>();

            var exsistingOrder = await orderrepo
                .GetAsync(new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId!));

            if (exsistingOrder != null)
                orderrepo.Delete(exsistingOrder);

            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

            // 5 . Create Order

            var order = new Models.Order(userEmail, address, orderItems, deliveryMethod, subtotal, basket.PaymentIntentId!);

            //  Save to Database

            await orderrepo
                        .AddAsync(order);
            await unitOfWork.SaveChangesAsync();
            // Map from Order to OrderResult and return
            return mapper.Map<OrderResult>(order);
        }
        #endregion

        private OrderItem CreateOrderItem(BasketItem item, Models.Product product)
        => new OrderItem(new ProductOrderItem(product.Id, product.Name, product.PictureUrl), item.Quantity, product.Price);

        #region Part 6 Order Service [ Get Delivery Methods ] 
        public async Task<IEnumerable<DeliveryMethodResult>> GetDeliveryMethodsAsync()
        {
            var deliveryMethods = await unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetAllAsync();
            return mapper.Map<IEnumerable<DeliveryMethodResult>>(deliveryMethods);
        }
        #endregion

        #region Part 7 Order Service & Order Specifications
        public async Task<OrderResult> GetOrderByIdAsync(Guid id)
        {
            var order = await unitOfWork.GetRepository<Models.Order, Guid>()
                 .GetAsync(new OrderWithIncludeSpecifications(id)) ?? throw new KeyNotFoundException(id.ToString());

            return mapper.Map<OrderResult>(order);
        }

        // Order.Email == email 
        public async Task<IEnumerable<OrderResult>> GetOrdersByEmailAsync(string email)
        {
            var orders = await unitOfWork.GetRepository<Models.Order, Guid>()
                 .GetAllAsync(new OrderWithIncludeSpecifications(email));

            return mapper.Map<IEnumerable<OrderResult>>(orders);
        }
        #endregion
    }

}
