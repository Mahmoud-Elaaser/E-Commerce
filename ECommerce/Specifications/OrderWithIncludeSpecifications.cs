using ECommerce.Models;

namespace ECommerce.Specifications
{
    internal class OrderWithIncludeSpecifications : BaseSpecification<Order>
    {
        public OrderWithIncludeSpecifications(Guid id)
            : base(o => o.Id == id)
        {
            AddInclude(o => o.DeliveryMethod);
            AddInclude(o => o.OrderItems);
        }

        public OrderWithIncludeSpecifications(string email)
           : base(o => o.UserEmail == email)
        {
            AddInclude(o => o.DeliveryMethod);
            AddInclude(o => o.OrderItems);

            AddOrderByDescending(o => o.OrderDate);
        }

    }
}
