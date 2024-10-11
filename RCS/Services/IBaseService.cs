using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Application.Interfaces
{
    public interface IBaseService<TContext, T> where TContext : DbContext where T : class, new()
    {
        Task<IEnumerable<T>> GetDataAsync();
        Task<T?> GetRecordAsync(Expression<Func<T, bool>> predicate);
        Task<int> AddAsync(T entity, Expression<Func<T, bool>> predicate);
        Task AddRangeAsync(IQueryable<T> entity);
        Task<bool> UpdateAsync(T entity, Expression<Func<T, bool>> predicate);
        Task DeleteAsync(T entity);
        Task DeleteByIdAsync(int id);
    }
}
