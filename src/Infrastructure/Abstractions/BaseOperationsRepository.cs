using Application.Abstractions.Repositories;
using Domain.Abstractions;
using Infrastructure.Core;

namespace Infrastructure.Abstractions;

internal abstract class BaseOperationsRepository<TEntity>(CoreDbContext dbContext) : Repository<TEntity>(dbContext),
	IAddOperations<TEntity>,
	IUpdateOperations<TEntity>,
	IDeleteOperations<TEntity>
	where TEntity : class, IEntity
{
	public virtual void Add(TEntity entity)
	{
		_entities.Add(entity);
	}
	public virtual void AddRange(ICollection<TEntity> entities)
	{
		_entities.AddRange(entities);
	}
	public virtual void Update(TEntity entity)
	{
		_entities.Update(entity);
	}
	public virtual void UpdateRange(ICollection<TEntity> entities)
	{
		_entities.UpdateRange(entities);
	}
	public virtual void Remove(TEntity entity)
	{
		if (_entities.Local.Any(e => e == entity) == false)
			_entities.Attach(entity);

		_entities.Remove(entity);
	}
}
