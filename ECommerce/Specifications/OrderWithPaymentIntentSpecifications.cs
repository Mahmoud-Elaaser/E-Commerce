using ECommerce.Models;

namespace ECommerce.Specifications
{
    internal class OrderWithPaymentIntentSpecifications : BaseSpecification<Order>
    {
        public OrderWithPaymentIntentSpecifications(string paymentIntentid) : base(o => o.PaymentIntentId == paymentIntentid)
        {

        }

    }
}
