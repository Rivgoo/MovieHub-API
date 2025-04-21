using Application.Abstractions.Repositories;
using Domain.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Abstractions;

internal abstract class OperationsRepository<TEntity, TId>(CoreDbContext dbContext) :
	BaseOperationsRepository<TEntity>(dbContext),
	IEntityOperations<TEntity, TId>
	where TId : notnull, IComparable<TId>
	where TEntity : class, IBaseEntity<TId>
{
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

	public override void Update(TEntity entity)
	{
		var trackedEntry = _dbContext.Set<TEntity>().Local.FirstOrDefault(e => e.Id.Equals(entity.Id));

		if (trackedEntry != null)
			_dbContext.Entry(trackedEntry).CurrentValues.SetValues(entity);
		else
		{
			_entities.Attach(entity);
			_entities.Entry(entity).State = EntityState.Modified;
		}
	}
}

internal abstract class OperationsRepository<TEntity>(CoreDbContext dbContext) :
	OperationsRepository<TEntity, int>(dbContext) where TEntity : BaseEntity<int>
{
}