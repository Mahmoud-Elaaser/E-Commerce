using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ECommerce.Specifications
{
    internal abstract class BaseSpecifications<TEntity, TKey> : ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        protected BaseSpecifications(Expression<Func<TEntity, bool>>? criteria)
        {
            Criteria = criteria;

        }
        public Expression<Func<TEntity, bool>>? Criteria { get; private set; }



        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = new();

        protected void AddIncludes(Expression<Func<TEntity, object>> includeExpression)
        {
            IncludeExpressions.Add(includeExpression);
        }



        public Expression<Func<TEntity, object>>? OrderBy { get; private set; }

        public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }
        protected void SetOrderBy(Expression<Func<TEntity, object>> orderByExpression)
           => OrderBy = orderByExpression;

        protected void SetOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression)
           => OrderByDescending = orderByDescExpression;

        public int Skip { get; private set; }

        public int Take { get; private set; }

        public bool IsPaginated { get; private set; }

        protected void ApplyPagination(int pageIndex, int PageSize)
        {
            IsPaginated = true;
            Take = PageSize;
            Skip = (pageIndex - 1) * PageSize;
        }


    }
}
