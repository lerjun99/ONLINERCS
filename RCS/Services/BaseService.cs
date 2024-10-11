using Microsoft.EntityFrameworkCore;
using Rock.Application.Interfaces;
using Rock.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Application.Services
{
    public class BaseService<TContext, T> : IBaseService<TContext, T> where TContext : DbContext where T : class, new()
    {
        private readonly IBaseRepository<TContext, T> _repository;

        public BaseService(IBaseRepository<TContext, T> repository) {
            _repository = repository;
        }
        public async Task<int> AddAsync(T entity, Expression<Func<T, bool>> predicate)
        {
            return await _repository.AddAsync(entity, predicate);
        }

        public async Task AddRangeAsync(IQueryable<T> entity)
        {
            await _repository.AddRangeAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            await _repository.DeleteAsync(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _repository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<T>> GetDataAsync()
        {
            return await _repository.GetDataAsync();
        }

        public async Task<T?> GetRecordAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.GetRecordAsync(predicate);
        }

        public async Task<bool> UpdateAsync(T entity, Expression<Func<T, bool>> predicate)
        {
            return await _repository.UpdateAsync(entity, predicate);
        }
    }
}
