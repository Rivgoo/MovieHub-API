using Domain.Abstractions;

namespace Application.Filters.Abstractions;

public interface ISorter<TEntity, TFilter>
	where TEntity : IEntity
	where TFilter : IFilter
{
	IQueryable<TEntity> GetSort(TFilter filter);
}