using Microsoft.EntityFrameworkCore;
using Solution.Services.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solution.Services.DAL.EF
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext RepositoryDbContext;
        protected DbSet<TEntity> RepositoryDbSet;

        public EFRepository(DbContext dbContext)
        {
            RepositoryDbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            RepositoryDbSet = RepositoryDbContext.Set<TEntity>();
        }

        public virtual void Add(TEntity entity)
        {
            RepositoryDbSet.Add(entity);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await RepositoryDbSet.AddAsync(entity);
        }

        public virtual TEntity Find(params object[] id)
        {
            return RepositoryDbSet.Find(id);
        }

        public virtual TEntity Find(int? id)
        {
            return RepositoryDbSet.Find(id);
        }

        public virtual async Task<TEntity> FindAsync(params object[] id)
        {
            return await RepositoryDbSet.FindAsync(id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return RepositoryDbSet.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await RepositoryDbSet.ToListAsync();
        }

        public virtual void Remove(TEntity entity)
        {
            RepositoryDbSet.Remove(entity);
        }

        public virtual TEntity Update(TEntity entity)
        {
            return RepositoryDbSet.Update(entity).Entity;
        }
    }
}