using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Infrastructure.Persistence.Repositories
{
    public interface IBaseRepository<TContext, T> where TContext : DbContext where T : class
    {
        Task<IQueryable<T>> GetDataAsync();
        Task<T?> GetRecordAsync(Expression<Func<T, bool>> predicate);
        Task<IQueryable<T>?> GetRecordsAsync(Expression<Func<T, bool>> predicate);
        Task<int> AddAsync(T entity, Expression<Func<T, bool>>? predicate);
        Task AddRangeAsync(IQueryable<T> entity);
        Task<bool> UpdateAsync(T entity, Expression<Func<T, bool>>? predicate);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IQueryable<T> entity);
        Task DeleteByIdAsync(int id);
    }
}
