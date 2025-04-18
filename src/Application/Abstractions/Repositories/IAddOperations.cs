using Domain.Abstractions;

namespace Application.Abstractions.Repositories;

public interface IAddOperations<TEntity> where TEntity : IEntity
{
	void Add(TEntity entity);

	void AddRange(ICollection<TEntity> entities);
}