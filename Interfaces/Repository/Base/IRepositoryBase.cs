using System.Linq.Expressions;
using Calibr8Fit.Api.Interfaces.Model;

namespace Calibr8Fit.Api.Interfaces.Repository.Base
{
    public interface IRepositoryBase<T, TKey>
        where T : class, IEntity<TKey>
        where TKey : notnull
    {
        ValueTask<T?> GetAsync(params object?[] keyValues);
        Task<List<T>> GetRangeAsync(IEnumerable<TKey> keys);
        Task<List<T>> GetAllAsync();
        Task<bool> KeyExistsAsync(params object?[] keyValues);
        Task<T?> AddAsync(T entity);
        Task<List<T>> AddRangeAsync(IEnumerable<T> entities);
        Task<T?> UpdateAsync(T entity);
        Task<T?> AddOrUpdateAsync(T entity);
        Task<List<T>> UpdateRangeAsync(IEnumerable<T> entities);
        Task<T?> DeleteAsync(params object?[] keyValues);
        Task<List<T>> DeleteRangeAsync(IEnumerable<TKey> keys);
        Task<bool> KeyExistsInHierarchyAsync(params object?[] keyValues);
        Task<List<TKey>> KeyRangeExistsInHierarchyAsync(IEnumerable<TKey> keys);

        Task<List<T>> QueryAsync(Func<IQueryable<T>, IQueryable<T>> configure, bool asNoTracking = true);
        Task<T?> QuerySingleAsync(Func<IQueryable<T>, IQueryable<T>> configure, bool asNoTracking = true);
        Task<List<TOut>> QueryProjectedAsync<TOut>(Func<IQueryable<T>, IQueryable<TOut>> configure, bool asNoTracking = true);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    }
}