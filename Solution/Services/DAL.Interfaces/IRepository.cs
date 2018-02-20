using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solution.Services.DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        TEntity Find(params object[] id);

        TEntity Find(int? id);

        Task<TEntity> FindAsync(params object[] id);

        void Add(TEntity entity);

        Task AddAsync(TEntity entity);

        TEntity Update(TEntity entity);

        void Remove(TEntity entity);
    }
}