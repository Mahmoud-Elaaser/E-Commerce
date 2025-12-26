using AutoMapper;
using ECommerce.DTOs.Basket;
using ECommerce.Enums;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using ECommerce.Settings;
using ECommerce.Specifications;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly StripeOptions _options;

        public PaymentService(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration,
            IOptions<StripeOptions> options)
        {
            _options = options.Value;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<CustomerBasketDto> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration.GetSection("StripeSetings")["SecretKey"];

            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null)
                throw new EmptyBasketException(basketId);

            /// Update product prices 
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Models.Product>()
                    .GetByIdAsync(item.Id);

                if (product == null)
                    throw new ProductNotFoundException(item.Id);

                item.Price = product.Price;
            }

            if (!basket.DeliveryMethodId.HasValue)
                throw new DeliveryMethodNotSelectedException(basketId);

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                .GetByIdAsync(basket.DeliveryMethodId.Value);

            if (deliveryMethod == null)
                throw new DeliveryMethodNotFoundException(basket.DeliveryMethodId.Value);

            var ShippingCost = deliveryMethod.Cost;
            var subTotal = basket.Items.Sum(i => i.Price * i.Quantity);
            /// Calculate total amount ( (* 100) => convert to cents for Stripe)
            var totalAmount = (long)((subTotal + ShippingCost) * 100);


            var stripeService = new PaymentIntentService();

            try
            {
                /// Create payment intent 
                if (string.IsNullOrEmpty(basket.PaymentIntentId))
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = totalAmount,
                        Currency = "usd",
                        PaymentMethodTypes = new List<string> { "card" }
                    };

                    var paymentIntent = await stripeService.CreateAsync(options);

                    /// Save payment intent details to basket
                    basket.PaymentIntentId = paymentIntent.Id;
                    basket.ClientSecret = paymentIntent.ClientSecret;

                }
                else
                {
                    /// Update payment intent 
                    var updateOptions = new PaymentIntentUpdateOptions
                    {
                        Amount = totalAmount
                    };

                    await stripeService.UpdateAsync(basket.PaymentIntentId, updateOptions);
                }
            }
            catch (StripeException ex)
            {

                if (string.IsNullOrEmpty(basket.PaymentIntentId))
                    throw new PaymentIntentCreationException(basketId, ex.Message, ex);
                else
                    throw new PaymentIntentUpdateException(basket.PaymentIntentId, ex.Message, ex);
            }

            await _basketRepository.CreateOrUpdateBasketAsync(basket);

            return _mapper.Map<CustomerBasketDto>(basket);
        }




        public async Task UpdateOrderPaymentStatusAsync(string json, string header)
        {
            var endpointSecret = _configuration.GetSection("StripeSetings")["EndpointSecret"];

            var stripeEvent = EventUtility.ParseEvent(json, throwOnApiVersionMismatch: false);


            stripeEvent = EventUtility.ConstructEvent(json, header, endpointSecret, throwOnApiVersionMismatch: false);
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    await UpdatePaymentIntentSucceededAsync(paymentIntent!.Id);
                    break;
                case EventTypes.PaymentIntentPaymentFailed:
                    await UpdatePaymentIntentFailedAsync(paymentIntent!.Id);
                    break;

                default:
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                    break;
            }
            throw new NotImplementedException();
        }

        private async Task UpdatePaymentIntentFailedAsync(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetAsyncWithSpec(spec);
            if (order == null)
                throw new OrderNotFoundException(paymentIntentId);

            order.OrderStatus = OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();

        }

        private async Task UpdatePaymentIntentSucceededAsync(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetAsyncWithSpec(spec);

            if (order == null)
                throw new OrderNotFoundException(paymentIntentId);

            order.OrderStatus = OrderStatus.PaymentReceived;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();

        }
    }
}