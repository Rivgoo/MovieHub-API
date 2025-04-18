using Domain.Abstractions;

namespace Application.Abstractions.Repositories;

public interface IDeleteOperations<TEntity> where TEntity : IEntity
{
	void Remove(TEntity entity);
}