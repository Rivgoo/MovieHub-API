using Application.Abstractions.Repositories;
using Domain.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Abstractions;

internal abstract class OperationsRepository<TEntity, TId>(CoreDbContext dbContext) :
	Repository<TEntity, TId>(dbContext),
	IEntityOperations<TEntity, TId>
	where TId : notnull, IComparable<TId>
	where TEntity : BaseEntity<TId>
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

	public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
	{
		return await _entities.AsNoTracking().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
	}
	public virtual TEntity GetById(TId id)
	{
		return _entities.AsNoTracking().Where(x => x.Id.Equals(id)).FirstOrDefault();
	}

	public virtual async Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _entities.AsNoTracking().ToListAsync(cancellationToken);
	}
	public virtual ICollection<TEntity> GetAll()
	{
		return [.. _entities.AsNoTracking()];
	}

	public virtual async Task<bool> ExistByIdAsync(TId id, CancellationToken cancellationToken = default)
	{
		return await _entities.AnyAsync(e => e.Id.Equals(id), cancellationToken);
	}
}

internal abstract class OperationsRepository<TEntity>(CoreDbContext dbContext) :
	OperationsRepository<TEntity, int>(dbContext) where TEntity : BaseEntity<int>
{
}