using Application.Abstractions.Repositories;
using Domain.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

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
		var trackedEntry = _dbContext.Set<TEntity>().Local.FirstOrDefault(e => e.Equals(entity));

		if (trackedEntry != null)
			_dbContext.Entry(trackedEntry).CurrentValues.SetValues(entity);
		else
		{
			_entities.Attach(entity);
			_entities.Entry(entity).State = EntityState.Modified;
		}
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
