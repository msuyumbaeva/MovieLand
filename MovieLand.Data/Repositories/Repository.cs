using Microsoft.EntityFrameworkCore;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Builders;
using MovieLand.Data.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MovieLand.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected AppDbContext _context;
        protected DbSet<T> DbSet { get; set; }

        public Repository(AppDbContext context) {
            _context = context;
            DbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(EntityQueryBuilder<T> queryBuilder) {
            var query = DbSet.AsNoTracking();

            foreach (var include in queryBuilder.Includes) 
                query = query.Include(include);

            if (queryBuilder.Filter != null) 
                query = query.Where(queryBuilder.Filter);

            if (queryBuilder.OrderBy != null)
                query = queryBuilder.OrderBy(query);

            if (queryBuilder.Offset.HasValue && queryBuilder.Offset > 0)
                query = query.Skip(queryBuilder.Offset.Value);

            if(queryBuilder.Limit.HasValue && queryBuilder.Limit > 0) {
                query = query.Take(queryBuilder.Limit.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync() {
            return await DbSet.CountAsync();
        }

        public async Task<T> AddAsync(T entity) {
            var entry = await DbSet.AddAsync(entity);
            return entry.Entity;
        }

        public void Update(T entity) {
            DbSet.Update(entity);
        }

        public void Delete(T entity) {
            DbSet.Remove(entity);
        }
    }
}
