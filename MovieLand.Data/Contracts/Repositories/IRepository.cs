using MovieLand.Data.Builders;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.Data.Contracts.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync(EntityQueryBuilder<T> queryBuilder);
        Task<int> CountAsync();
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
