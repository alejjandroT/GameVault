using GameVault.Domain.Common;

namespace GameVault.Application.Abstractions.Persistence;

public interface IGenericRepository<TEntity, TId>
    where TEntity : AuditableEntity<TId>
    where TId : notnull
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
}