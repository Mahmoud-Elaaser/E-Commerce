using ECommerce.Models;

namespace ECommerce.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        public Task<int> SaveChangesAsync();

        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;


    }
}
