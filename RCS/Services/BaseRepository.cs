using Microsoft.EntityFrameworkCore;
using Rock.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Infrastructure.Persistence.Repositories
{
    public class BaseRepository<TContext, T> : IBaseRepository<TContext, T>
        where TContext : DbContext
        where T : class, new()
    {
        private readonly TContext _context;

        public BaseRepository(TContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<T>> GetDataAsync()
        {
            return await Task.Run(() =>
            {
                return _context.Set<T>();
            });
        }

        public async Task<T?> GetRecordAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>()
                .Where(predicate)
                .FirstOrDefaultAsync();
        }

        public async Task<IQueryable<T>?> GetRecordsAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() =>
            {
                return _context.Set<T>()
                .Where(predicate);
            });
        }

        public async Task<int> AddAsync(T entity, Expression<Func<T, bool>>? predicate)
        {
            //check if record exist
            var isExist = predicate != null ? _context.Set<T>().Where(predicate).Any() : false;

            // if record is not exist
            if (!isExist)
            {
                await _context.Set<T>().AddAsync(entity);
                await SaveChangesAsync();

                // return entity Id
                var entityType = typeof(T);
                var propertyValue = entityType.GetProperty("Id")?.GetValue(entity);
                if(propertyValue != null)
                    return (int)propertyValue;
                else
                    return 0;
            }
            else
                return 0;
        }

        public async Task AddRangeAsync(IQueryable<T> entity)
        {
            await _context.Set<T>().AddRangeAsync(entity);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IQueryable<T> entity)
        {
            _context.Set<T>().RemoveRange(entity);
            await SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var result = await _context.Set<T>().FindAsync(id);
            if (result != null)
            {
                _context.Set<T>().Remove(result);
                await SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateAsync(T entity, Expression<Func<T, bool>>? predicate)
        {
            var isExist = predicate != null ? _context.Set<T>().Where(predicate).Any() : false;

            if (!isExist)
            {
                _context.Set<T>().Update(entity);
                await SaveChangesAsync();
                return true;
            }
            else
                return false;
        }

        protected async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
