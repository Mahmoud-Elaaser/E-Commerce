using ECommerce.Models;

namespace ECommerce.Specifications
{
    internal class OrderWithIncludeSpecifications : BaseSpecifications<Order, Guid>
    {
        public OrderWithIncludeSpecifications(Guid id)
            : base(o => o.Id == id)
        {
            AddIncludes(o => o.DeliveryMethod);
            AddIncludes(o => o.OrderItems);
        }

        public OrderWithIncludeSpecifications(string email)
           : base(o => o.UserEmail == email)
        {
            AddIncludes(o => o.DeliveryMethod);
            AddIncludes(o => o.OrderItems);

            SetOrderBy(o => o.OrderDate);
        }

    }
}
