using Domain.Abstractions;

namespace Application.Abstractions.Repositories;

public interface IGetOperations<TEntity, TId> 
	where TEntity : IEntity
	where TId : IComparable<TId>
{
	Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
	ICollection<TEntity> GetAll();

	TEntity GetById(TId id);
	Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}