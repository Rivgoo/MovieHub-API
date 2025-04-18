using Domain.Abstractions;

namespace Application.Abstractions.Repositories;

public interface IUpdateOperations<TEntity> where TEntity : IEntity
{
	void Update(TEntity entity);
	void UpdateRange(ICollection<TEntity> entities);
}