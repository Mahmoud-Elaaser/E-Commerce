using AutoMapper;
using ECommerce.DTOs.Basket;
using ECommerce.Enums;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using ECommerce.Specifications;
using Stripe;

namespace ECommerce.Services.Implementations
{
    internal class PaymentService(IConfiguration configuration
        , IBasketRepository basketRepository, IUnitOfWork unitOfWork, IMapper mapper) : IPaymentService
    {
        #region Part 2 Payment Service Create Payment Intent
        // 0. Install stripe Package
        // 1. Set Up Stripe Api key [Secret Key]
        //  2. Get Basket by Id From Repo
        // 3. Validate Basket Items and Prices == Product.Price to Get real Price from Data base
        // 4. Get Delivery Method And Shipping Price
        // 5 . Retrive Delivery Method From Db and assign Price Of Basket [Shipping Price] = DeliveryMethod.ShippingPrice
        // 6. Total = SubTotal + ShippingPrice [(Items.Price * Items.Quantity)  + basket.ShppingPrice] ==> Dolar --> Cent * 100
        // 7. Save Changes to Basket 
        // 8. Mapping Basket --> BasketDTO 
        public async Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            // Configure Stripe Api Key Using Secret Key From AppSettings
            StripeConfiguration.ApiKey = configuration.GetSection("StripeSetings")["SecretKey"];

            // Retrive Basket Using BasketId --> From Basket Repo

            var basket = await basketRepository.GetBasketAsync(basketId) ?? throw new KeyNotFoundException(basketId);

            foreach (var item in basket.Items)
            {
                // Get Product From Db ==>  to Validate Price ( Validate Price that End User Enter was Correct   
                var product = await unitOfWork.GetRepository<Models.Product, int>()
                                    .GetAsync(item.Id) ?? throw new KeyNotFoundException(item.Id.ToString());

                // Update item.price == product.Price in product table in database [ Real Price ]
                item.Price = product.Price;


            }

            // check if DeliveryMethod Is Selected Or not
            if (!basket.DeliveryMethodId.HasValue) throw new Exception("Delivery Method Is Not Selected");

            // Retrive Delivery Method From Db
            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>()
                                        .GetAsync(basket.DeliveryMethodId.Value)
                                        ?? throw new KeyNotFoundException(basket.DeliveryMethodId.Value.ToString());
            // Assign Shipping Price to Basket
            basket.ShippingPrice = deliveryMethod.Cost;

            // Calculate Total Amount [ SubTotal + ShippingPrice ]
            var totalAmount = (long)((basket.Items.Sum(i => i.Price * i.Quantity) + basket.ShippingPrice) * 100);

            var stripeService = new PaymentIntentService();

            // If You want To Create Or Update PaymentIntent    

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                // Create New Payment Intent
                var options = new PaymentIntentCreateOptions
                {
                    Amount = totalAmount,
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                var paymentIntent = await stripeService.CreateAsync(options);
                // Assign PaymentIntentId And ClientSecret to Basket
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                // Update Payment Intent
                // 1. Admin Changes Product Prices [database]
                // 2 . User Change Delivery Method
                // 3. User Add Or Remove Items From Basket

                var updateoptions = new PaymentIntentUpdateOptions
                {
                    Amount = totalAmount
                };
                await stripeService.UpdateAsync(basket.PaymentIntentId, updateoptions);
            }
            // Save Changes to Basket
            await basketRepository.CreateOrUpdateBasketAsync(basket);

            // Map Basket --> BasketDTO
            return mapper.Map<BasketDTO>(basket);

        }

        public async Task UpdateOrderPaymentStatusAsync(string json, string header)
        {
            var endpointSecret = configuration.GetSection("StripeSetings")["EndPointSecret"];

            var stripeEvent = EventUtility.ParseEvent(json, throwOnApiVersionMismatch: false);


            stripeEvent = EventUtility.ConstructEvent(json, header, endpointSecret, throwOnApiVersionMismatch: false);
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            // Handle the event
            // If on SDK version < 46, use class Events instead of EventTypes
            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    await UpdatePaymentIntentSucceeded(paymentIntent!.Id);
                    break;
                case EventTypes.PaymentIntentPaymentFailed:
                    await UpdatePaymentIntentFailed(paymentIntent!.Id);
                    break;

                default:
                    // Unexpected event type
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                    break;
            }



            throw new NotImplementedException();
        }

        private async Task UpdatePaymentIntentFailed(string PaymentIntentId)
        {
            var orderRepo = unitOfWork.GetRepository<Order, Guid>();


            var order = await orderRepo
                              .GetAsync(new OrderWithPaymentIntentSpecifications(PaymentIntentId))
                              ?? throw new Exception();

            order.PaymentStatus = OrderStatus.PaymentFailed;

            orderRepo.Update(order);
            await unitOfWork.SaveChangesAsync();

        }

        private async Task UpdatePaymentIntentSucceeded(string PaymentIntentId)
        {
            var orderRepo = unitOfWork.GetRepository<Order, Guid>();


            var order = await orderRepo
                              .GetAsync(new OrderWithPaymentIntentSpecifications(PaymentIntentId))
                              ?? throw new Exception();

            order.PaymentStatus = OrderStatus.PaymentReceived;

            orderRepo.Update(order);
            await unitOfWork.SaveChangesAsync();
        }

        #endregion
    }
}
