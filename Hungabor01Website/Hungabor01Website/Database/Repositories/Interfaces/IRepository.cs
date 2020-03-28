using Hungabor01Website.Database.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hungabor01Website.Database.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public void Initialize(AppDbContext context);

        public TEntity Get(int id);

        public Task<TEntity> GetAsync(int id);

        public IEnumerable<TEntity> GetAll();

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        public void Add(TEntity entity);

        public Task AddAsync(TEntity entity);

        public void AddRange(IEnumerable<TEntity> Entities);

        public Task AddRangeAsync(IEnumerable<TEntity> Entities);

        public void Remove(TEntity entity);

        public void RemoveRange(IEnumerable<TEntity> Entities);
    }
}