using System.Linq.Expressions;

namespace Byohar.Application.Interfaces.Persistence;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> Entities(bool isUserLoggedIn = true);
    IEnumerable<T> GetAll();
    IQueryable<T> GetAllAsQueryable();
    T GetById<TId>(TId id);
    Task<T> GetByIdAsync<TId>(TId id);
    T Add(T entity);
    void Update(T entity);
    void Delete<TId>(TId id);
    void Delete<TId>(TId id, bool permanentDelete = true);
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    void AddRange(IEnumerable<T> entities);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
    void UpdateRange(IEnumerable<T> entities);
    void DeleteRange(IEnumerable<T> entities, bool permanentDelete = false);
}