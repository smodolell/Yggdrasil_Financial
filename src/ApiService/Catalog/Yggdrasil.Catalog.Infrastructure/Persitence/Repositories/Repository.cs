using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Yggdrasil.Catalog.Application.Common.Interfaces;
using Yggdrasil.Catalog.Domain.Interfaces;

namespace Yggdrasil.Catalog.Infrastructure.Persitence.Repositories;

public abstract class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    private readonly bool _isAuditable;

    public Repository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<TEntity>();
        _isAuditable = typeof(IAuditable).IsAssignableFrom(typeof(TEntity));
    }


    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id! }, cancellationToken);

        if (_isAuditable && entity is IAuditable auditable && auditable.IsDeleted)
            return null;

        return entity;
    }

    public async Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await ApplyAuditFilters(_dbSet).ToListAsync();
    }

    public async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return await ApplyAuditFilters(_dbSet).ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).CountAsync(cancellationToken);
    }

    public async Task<bool> AnySpecAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).AnyAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (_isAuditable && entity is IAuditable auditable)
        {
            auditable.CreatedAt = DateTime.UtcNow;
            auditable.IsDeleted = false;
        }

        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (_isAuditable && entity is IAuditable auditable)
        {
            auditable.UpdatedAt = DateTime.UtcNow;
        }

        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;

        await Task.CompletedTask;
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (_isAuditable && entity is IAuditable auditable)
        {
            // Soft delete
            auditable.IsDeleted = true;
            auditable.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(entity, cancellationToken);
        }
        else
        {
            // Hard delete
            _dbSet.Remove(entity);
        }
    }

    public async Task DeleteAsync(TKey id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            await DeleteAsync(entity);
        }
    }

    public IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        var query = ApplyAuditFilters(_dbSet);
        return SpecificationEvaluator.Default.GetQuery(query, spec);
    }

    private IQueryable<TEntity> ApplyAuditFilters(IQueryable<TEntity> query)
    {
        if (_isAuditable)
        {
            query = query.Where(e => !EF.Property<bool>(e, "IsDeleted"));
        }
        return query;
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate,cancellationToken);
    }
}