using ECommerce.Models;

namespace ECommerce.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {

        // Get All 
        Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false);
        //Get By Id
        Task<TEntity?> GetAsync(TKey id);

        #region Specifications
        // Get All 
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications);
        //Get By Id
        Task<TEntity?> GetAsync(ISpecifications<TEntity, TKey> specifications);
        Task<int> CountAsync(ISpecifications<TEntity, TKey> specifications);

        #endregion
        //Create
        Task AddAsync(TEntity entity);
        //Update
        void Update(TEntity entity);
        //Delete
        void Delete(TEntity entity);
    }
}
