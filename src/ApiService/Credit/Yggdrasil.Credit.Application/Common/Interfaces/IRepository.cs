using System.Linq.Expressions;
using Yggdrasil.Credit.Domain.Interfaces;

namespace Yggdrasil.Credit.Application.Common.Interfaces;

public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnySpecAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TKey id);
    IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec);
}
