using ECommerce.Models;

namespace ECommerce.Specifications
{
    internal class OrderWithPaymentIntentSpecifications : BaseSpecifications<Order, Guid>
    {
        public OrderWithPaymentIntentSpecifications(string paymentIntentid) : base(o => o.PaymentIntentId == paymentIntentid)
        {

        }
    }
}
