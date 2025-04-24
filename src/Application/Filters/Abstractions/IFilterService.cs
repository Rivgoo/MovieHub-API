using Application.Results;
using Domain.Abstractions;
using System.Linq.Expressions;

namespace Application.Filters.Abstractions;

public interface IFilterService<TEntity, TFilter>
	where TEntity : class, IEntity
	where TFilter : IFilter
{
	IQueryable<TResult> GetQuery<TResult>(Expression<Func<TEntity, TResult>> selector)
		where TResult : class;
	IQueryable<TResult> GetQuery<TResult, TSelector>()
		where TResult : class
		where TSelector : ISelector<TEntity, TResult>;

	IFilterService<TEntity, TFilter> AddFilter(TFilter filter);
	IFilterService<TEntity, TFilter> AddSorter<TSorter>() where TSorter : ISorter<TEntity, TFilter>;
	IFilterService<TEntity, TFilter> SetPageSize(int pageSize);
	IFilterService<TEntity, TFilter> SplitQuery(bool splitQuery = true);

	Task<Result<PaginatedList<TResult>>> ApplyAsync<TResult, TSelector>(CancellationToken cancellationToken = default)
		where TResult : class
		where TSelector : ISelector<TEntity, TResult>;

	Task<Result<PaginatedList<TResult>>> ApplyAsync<TResult>(
		Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
		where TResult : class;

	Task<Result<PaginatedList<TResult>>> ApplyWithUnionAsync<TResult, TSelector>(
		IQueryable<TResult> union,
		List<QueryableOrder> resultOrder = null,
		CancellationToken cancellationToken = default)
		where TResult : class
		where TSelector : ISelector<TEntity, TResult>;

	Task<Result<PaginatedList<TResult>>> ApplyWithUnionAsync<TResult>(
		IQueryable<TResult> union, 
		Expression<Func<TEntity, TResult>> selector,
		List<QueryableOrder> resultOrder = null,
		CancellationToken cancellationToken = default)
		where TResult : class;
}