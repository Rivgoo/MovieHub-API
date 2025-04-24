using Domain.Abstractions;

namespace Application.Filters.Abstractions;

public interface ISelector<TEntity, TResult>
	where TEntity : IEntity
{
	IQueryable<TResult> Select(IQueryable<TEntity> source);
}