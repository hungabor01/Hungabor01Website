using Hungabor01Website.Database.Core;
using Hungabor01Website.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hungabor01Website.Database.Repositories.Classes
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected AppDbContext Context { get; set; }

        public void Initialize(AppDbContext context)
        {
            Context = context;
        }

        public TEntity Get(int id) =>
            Context.Set<TEntity>().Find(id);

        public async Task<TEntity> GetAsync(int id) =>
            await Context.Set<TEntity>().FindAsync(id);

        public IEnumerable<TEntity> GetAll() =>
            Context.Set<TEntity>().ToList();

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) => 
            Context.Set<TEntity>().Where(predicate);

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate) =>
            Context.Set<TEntity>().SingleOrDefault(predicate);

        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) => 
            await Context.Set<TEntity>().SingleOrDefaultAsync(predicate);

        public void Add(TEntity entity) =>
            Context.Set<TEntity>().Add(entity);

        public async Task AddAsync(TEntity entity) =>
            await Context.Set<TEntity>().AddAsync(entity);

        public void AddRange(IEnumerable<TEntity> entities) => 
            Context.Set<TEntity>().AddRange(entities);

        public async Task AddRangeAsync(IEnumerable<TEntity> entities) =>
            await Context.Set<TEntity>().AddRangeAsync(entities);

        public void Remove(TEntity entity) => 
            Context.Set<TEntity>().Remove(entity);

        public void RemoveRange(IEnumerable<TEntity> entities) => 
            Context.Set<TEntity>().RemoveRange(entities);
    }
}
