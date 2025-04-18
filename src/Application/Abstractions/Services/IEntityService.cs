using Application.Results;
using Domain.Abstractions;

namespace Application.Abstractions.Services;

public interface IEntityService<TEntity, TId>
	where TEntity : BaseEntity<TId>
	where TId : IComparable<TId>
{
	Task<Result<TEntity>> GetByIdAsync(TId entityId, CancellationToken cancellationToken = default);
	Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

	Task<bool> ExistsByIdAsync(TId? entityId, CancellationToken cancellationToken = default);
	Task<Result> VerifyExistsByIdAsync(TId? entityId, CancellationToken cancellationToken = default);

	Task<Result<TEntity>> CreateAsync(TEntity newEntity);
	Task<Result<TEntity>> UpdateAsync(TEntity changedEntity);
	Task<Result> DeleteByIdAsync(TId entityId);
}