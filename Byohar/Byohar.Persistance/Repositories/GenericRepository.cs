using Byohar.Application.Interfaces.Persistance;
using Byohar.Domain.Common;
using Byohar.Persistance.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Byohar.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GenericRepository() { }

    public GenericRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _context = db;
        _httpContextAccessor = httpContextAccessor;
    }

    private DbSet<T> _entities => _context.Set<T>();

    public IQueryable<T> Entities(bool applyTenantFilter = true)
    {
        var entityType = typeof(T);
        var query = _context.Set<T>().AsQueryable();
        var tenantId = GetTenantId();

        if (typeof(IFullAuditableEntity).IsAssignableFrom(entityType))
        {
            query = query.OfType<IFullAuditableEntity>()
                         .Where(e => !e.IsDeleted && (!applyTenantFilter || e.TenantId == tenantId))
                         .OrderByDescending(x => x.CreatedOn)
                         .Cast<T>();
        }
        else if (typeof(IAuditableEntity).IsAssignableFrom(entityType))
        {
            query = query.OfType<IAuditableEntity>()
                         .Where(e => !applyTenantFilter || e.TenantId == tenantId)
                         .OrderByDescending(x => x.CreatedOn)
                         .Cast<T>();
        }
        else if (typeof(IEntity).IsAssignableFrom(entityType) && applyTenantFilter)
        {
            query = query.OfType<IEntity>()
                         .Where(e => e.TenantId == tenantId)
                         .Cast<T>();
        }

        return query;
    }

    public IQueryable<T> GetAllAsQueryable()
    {
        if (typeof(T).IsSubclassOf(typeof(IFullAuditableEntity)))
        {
            return _entities.OfType<IFullAuditableEntity>().Where(e => !e.IsDeleted && e.TenantId == GetTenantId()).Cast<T>().AsQueryable();
        }
        else
        {
            return _entities.AsQueryable();
        }
    }

    public IEnumerable<T> GetAll()
    {
        var tenantId = GetTenantId();

        if (typeof(IFullAuditableEntity).IsAssignableFrom(typeof(T)))
        {
            return _entities.OfType<IFullAuditableEntity>()
                            .Where(e => !e.IsDeleted && e.TenantId == tenantId)
                            .Cast<T>()
                            .ToList();
        }
        else if (typeof(IAuditableEntity).IsAssignableFrom(typeof(T)))
        {
            return _entities.OfType<IAuditableEntity>()
                            .Where(e => e.TenantId == tenantId)
                            .Cast<T>()
                            .ToList();
        }
        else if (typeof(IEntity).IsAssignableFrom(typeof(T)))
        {
            return _entities.OfType<IEntity>()
                            .Where(e => e.TenantId == tenantId)
                            .Cast<T>()
                            .ToList();
        }
        else
        {
            return _entities.ToList();
        }
    }

    public T GetById<TId>(TId id)
    {
        var entity = _entities.Find(id);

        if (entity == null) return default;

        if (entity is IFullAuditableEntity fullAuditableEntity)
        {
            return !fullAuditableEntity.IsDeleted && fullAuditableEntity.TenantId == GetTenantId() ? entity : default;
        }
        else if (entity is IAuditableEntity auditableEntity)
        {
            return auditableEntity.TenantId == GetTenantId() ? entity : default;
        }
        else if (entity is IEntity baseEntity)
        {
            return baseEntity.TenantId == GetTenantId() ? entity : default;
        }
        else
        {
            return entity;
        }
    }

    public async Task<T> GetByIdAsync<TId>(TId id)
    {
        var entity = await _entities.FindAsync(id);

        if (entity == null) return default;

        if (entity is IFullAuditableEntity fullAuditableEntity)
        {
            return !fullAuditableEntity.IsDeleted && fullAuditableEntity.TenantId == GetTenantId() ? entity : default;
        }
        else if (entity is IAuditableEntity auditableEntity)
        {
            return auditableEntity.TenantId == GetTenantId() ? entity : default;
        }
        else if (entity is IEntity baseEntity)
        {
            return baseEntity.TenantId == GetTenantId() ? entity : default;
        }
        else
        {
            return entity;
        }
    }

    public T Add(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var response = _entities.Add(entity);

        return response.Entity;
    }

    public void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete<TId>(TId id)
    {
        T entityToDelete = _entities.Find(id);

        if (entityToDelete != null)
        {
            if (entityToDelete is IFullAuditableEntity deletableEntity)
            {
                if (deletableEntity.IsDeleted) return;

                deletableEntity.IsDeleted = true;

                _context.Entry(deletableEntity).State = EntityState.Modified;
            }
            else
            {
                _entities.Remove(entityToDelete);
            }
        }
    }

    public void Delete<TId>(TId id, bool permanentDelete = true)
    {
        if (permanentDelete)
        {
            T entityToDelete = _entities.Find(id);

            if (entityToDelete != null)
            {
                _entities.Remove(entityToDelete);
            }
        }
        else
        {
            Delete(id);
        }
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        if (typeof(T).IsSubclassOf(typeof(IFullAuditableEntity)))
        {
            var entity = await _entities.FirstOrDefaultAsync(predicate);

            if (entity is IFullAuditableEntity baseEntity)
            {
                if (!baseEntity.IsDeleted)
                {
                    return entity;
                }
            }
        }
        else
        {
            return await _entities.FirstOrDefaultAsync(predicate);
        }

        return default;
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _entities.UpdateRange(entities);
    }

    public void DeleteRange(IEnumerable<T> entities, bool permanentDelete = false)
    {
        if (permanentDelete)
        {
            _entities.RemoveRange(entities);
        }
        else
        {
            foreach (var entityToDelete in entities)
            {
                if (entityToDelete is IFullAuditableEntity deletableEntity)
                {
                    if (deletableEntity.IsDeleted) continue;

                    deletableEntity.IsDeleted = true;

                    _context.Entry(deletableEntity).State = EntityState.Modified;
                }
                else
                {
                    _entities.Remove(entityToDelete);
                }
            }
        }
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _entities.AddRange(entities);
    }

    public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        return _entities.AddRangeAsync(entities, cancellationToken);
    }

    private Guid? GetTenantId()
    {
        if (_httpContextAccessor.HttpContext?.Items.TryGetValue("TenantId", out var tenantIdValue) == true)
        {
            if (Guid.TryParse(tenantIdValue?.ToString(), out var tenantId))
            {
                return tenantId;
            }
        }

        return null;
    }

}
