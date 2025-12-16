using AutoMapper;
using ECommerce.DTOs.Basket;
using ECommerce.Enums;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using ECommerce.Specifications;
using Stripe;

namespace ECommerce.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IConfiguration configuration,
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PaymentService> logger)
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            var secretKey = _configuration.GetSection("StripeSettings")["SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                throw new StripeConfigurationException("SecretKey");

            StripeConfiguration.ApiKey = secretKey;

            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null)
                throw new EmptyBasketException(basketId);

            /// Update product prices from database
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

            basket.ShippingPrice = deliveryMethod.Cost;

            /// Calculate total amount (convert to cents for Stripe)
            var totalAmount = (long)((basket.Items.Sum(i => i.Price * i.Quantity) + basket.ShippingPrice) * 100);

            var stripeService = new PaymentIntentService();

            try
            {
                if (string.IsNullOrEmpty(basket.PaymentIntentId))
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = totalAmount,
                        Currency = "usd",
                        PaymentMethodTypes = new List<string> { "card" }
                    };

                    var paymentIntent = await stripeService.CreateAsync(options);

                    basket.PaymentIntentId = paymentIntent.Id;
                    basket.ClientSecret = paymentIntent.ClientSecret;

                    _logger.LogInformation(
                        "Created payment intent {PaymentIntentId} for basket {BasketId}",
                        paymentIntent.Id, basketId);
                }
                else
                {
                    var updateOptions = new PaymentIntentUpdateOptions
                    {
                        Amount = totalAmount
                    };

                    await stripeService.UpdateAsync(basket.PaymentIntentId, updateOptions);

                    _logger.LogInformation(
                        "Updated payment intent {PaymentIntentId} for basket {BasketId}",
                        basket.PaymentIntentId, basketId);
                }
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex,
                    "Stripe API error while processing payment intent for basket {BasketId}", basketId);

                if (string.IsNullOrEmpty(basket.PaymentIntentId))
                    throw new PaymentIntentCreationException(basketId, ex.Message, ex);
                else
                    throw new PaymentIntentUpdateException(basket.PaymentIntentId, ex.Message, ex);
            }

            await _basketRepository.CreateOrUpdateBasketAsync(basket);

            return _mapper.Map<BasketDTO>(basket);
        }

        public async Task UpdateOrderPaymentStatusAsync(string json, string header)
        {
            var endpointSecret = _configuration.GetSection("StripeSettings")["EndpointSecret"];
            if (string.IsNullOrEmpty(endpointSecret))
                throw new StripeConfigurationException("EndpointSecret");

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    header,
                    endpointSecret,
                    throwOnApiVersionMismatch: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to construct Stripe webhook event");
                throw new InvalidPaymentWebhookException("Failed to verify webhook signature", ex);
            }

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null)
            {
                _logger.LogWarning("Webhook event data is not a PaymentIntent. Type: {EventType}",
                    stripeEvent.Type);
                throw new InvalidPaymentWebhookException(
                    $"Expected PaymentIntent but received {stripeEvent.Data.Object?.GetType().Name}");
            }

            try
            {
                switch (stripeEvent.Type)
                {
                    case EventTypes.PaymentIntentSucceeded:
                        _logger.LogInformation(
                            "Processing payment success for PaymentIntent {PaymentIntentId}",
                            paymentIntent.Id);
                        await UpdatePaymentIntentSucceededAsync(paymentIntent.Id);
                        break;

                    case EventTypes.PaymentIntentPaymentFailed:
                        _logger.LogWarning(
                            "Processing payment failure for PaymentIntent {PaymentIntentId}",
                            paymentIntent.Id);
                        await UpdatePaymentIntentFailedAsync(paymentIntent.Id);
                        break;

                    default:
                        _logger.LogInformation(
                            "Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                        break;
                }
            }
            catch (OrderNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating order payment status for PaymentIntent {PaymentIntentId}",
                    paymentIntent.Id);
                throw new InvalidPaymentWebhookException(
                    $"Failed to update order status for payment intent {paymentIntent.Id}", ex);
            }
        }

        private async Task UpdatePaymentIntentFailedAsync(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetAsyncWithSpec(spec);
            if (order == null)
                throw new OrderNotFoundException(paymentIntentId);

            order.PaymentStatus = OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Updated order {OrderId} status to PaymentFailed", order.Id);
        }

        private async Task UpdatePaymentIntentSucceededAsync(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetAsyncWithSpec(spec);

            if (order == null)
                throw new OrderNotFoundException(paymentIntentId);

            order.PaymentStatus = OrderStatus.PaymentReceived;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Updated order {OrderId} status to PaymentReceived", order.Id);
        }
    }
}