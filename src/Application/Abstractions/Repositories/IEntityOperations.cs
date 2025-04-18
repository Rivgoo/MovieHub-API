using Domain.Abstractions;

namespace Application.Abstractions.Repositories;

public interface IEntityOperations<TEntity, TId> : 
	IAddOperations<TEntity>, 
	IGetOperations<TEntity, TId>, 
	IUpdateOperations<TEntity>, 
	IDeleteOperations<TEntity>, 
	IExistByIdOperation<TId> 
	where TEntity : IEntity
	where TId : notnull, IComparable<TId>
{
}