using Application.Filters.Abstractions;
using Domain.Abstractions;
using Infrastructure.Core;

namespace Infrastructure.Filters.Abstractions;

internal abstract class BaseSelector<TEntity, TResult>(CoreDbContext dbContext) : ISelector<TEntity, TResult>
	where TEntity : class, IEntity
{
	protected readonly CoreDbContext _dbContext = dbContext;

	public abstract IQueryable<TResult> Select(IQueryable<TEntity> source);
}