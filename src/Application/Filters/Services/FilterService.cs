using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using Application.Filters.Abstractions;
using Domain.Abstractions;
using Application.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Filters.Services;

internal class FilterService<TEntity, TFilter>(
	IServiceProvider serviceProvider) : IFilterService<TEntity, TFilter>
	where TEntity : class, IEntity
	where TFilter : IFilter
{
	private readonly IServiceProvider _serviceProvider = serviceProvider;

	private TFilter _filter;
	private ISorter<TEntity, TFilter> _sorter;

	private int _pageSize = 0;
	private bool _splitQuery = false;

	#region Builder
	public IFilterService<TEntity, TFilter> AddFilter(TFilter filter)
	{
		_filter = filter;
		return this;
	}
	public IFilterService<TEntity, TFilter> AddSorter<TSorter>()
		where TSorter : ISorter<TEntity, TFilter>
	{
		_sorter = _serviceProvider.GetRequiredService<TSorter>();
		return this;
	}
	public IFilterService<TEntity, TFilter> SetPageSize(int pageSize)
	{
		_pageSize = Math.Clamp(pageSize, 1, 10000000);
		return this;
	}
	public IFilterService<TEntity, TFilter> SplitQuery(bool splitQuery = true)
	{
		_splitQuery = splitQuery;
		return this;
	}
	#endregion

	public IQueryable<TResult> GetQuery<TResult>(Expression<Func<TEntity, TResult>> selector)
	where TResult : class
	{
		if (_filter == null)
			throw new ArgumentNullException(nameof(_filter));

		_sorter ??= _serviceProvider.GetRequiredService<ISorter<TEntity, TFilter>>();

		var query = _sorter.GetSort(_filter);

		query = _splitQuery ? query.AsSplitQuery() : query;

		query = GetOrderedQuery(query, _filter.GetOrders());

		var result = query.Select(selector);

		return result;
	}
	public IQueryable<TResult> GetQuery<TResult, TSelector>()
		where TResult : class
		where TSelector : ISelector<TEntity, TResult>
	{
		if (_filter == null)
			throw new ArgumentNullException(nameof(_filter));

		_sorter ??= _serviceProvider.GetRequiredService<ISorter<TEntity, TFilter>>();

		var query = _sorter.GetSort(_filter);

		query = _splitQuery ? query.AsSplitQuery() : query;

		query = GetOrderedQuery(query, _filter.GetOrders());

		return _serviceProvider.GetRequiredService<TSelector>().Select(query);
	}

	public async Task<Result<PaginatedList<TResult>>> ApplyAsync<TResult, TSelector>(CancellationToken cancellationToken = default)
		where TResult : class
		where TSelector : ISelector<TEntity, TResult>
	{
		if (_filter == null) throw new ArgumentNullException(nameof(_filter));

		if (_pageSize == 0) throw new ArgumentNullException(nameof(_pageSize));

		_sorter ??= _serviceProvider.GetRequiredService<ISorter<TEntity, TFilter>>();

		var query = _sorter.GetSort(_filter);

		var count = await PaginatedList<TEntity>.CountAsync(query, cancellationToken);

		query = GetOrderedQuery(query, _filter.GetOrders());
		query = _splitQuery ? query.AsSplitQuery() : query;

		var resultQuery = _serviceProvider.GetRequiredService<TSelector>().Select(query);

		var result = (await PaginatedList<TResult>.CreateAsync(resultQuery, _filter.PageIndex, _pageSize, cancellationToken))
			.SetTotalCount(count);

		return Result<PaginatedList<TResult>>.Ok(result);
	}
	public async Task<Result<PaginatedList<TResult>>> ApplyAsync<TResult>(
		Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
		where TResult : class
	{
		if (_filter == null) throw new ArgumentNullException(nameof(_filter));

		if (_pageSize == 0) throw new ArgumentNullException(nameof(_pageSize));

		_sorter ??= _serviceProvider.GetRequiredService<ISorter<TEntity, TFilter>>();

		var query = _sorter.GetSort(_filter);

		var count = await PaginatedList<TEntity>.CountAsync(query, cancellationToken);

		query = GetOrderedQuery(query, _filter.GetOrders());

		var resultQuery = (_splitQuery ? query.AsSplitQuery() : query).Select(selector);

		var result = (await PaginatedList<TResult>.CreateAsync(resultQuery, _filter.PageIndex, _pageSize, cancellationToken))
			.SetTotalCount(count);

		return Result<PaginatedList<TResult>>.Ok(result);
	}

	public async Task<Result<PaginatedList<TResult>>> ApplyWithUnionAsync<TResult, TSelector>(
		IQueryable<TResult> union,
		List<QueryableOrder> resultOrder = null,
		CancellationToken cancellationToken = default)
		where TResult : class
		where TSelector : ISelector<TEntity, TResult>
	{
		if (_filter == null) throw new ArgumentNullException(nameof(_filter));

		if (_pageSize == 0) throw new ArgumentNullException(nameof(_pageSize));

		_sorter ??= _serviceProvider.GetRequiredService<ISorter<TEntity, TFilter>>();

		var query = _sorter.GetSort(_filter);

		query = GetOrderedQuery(query, _filter.GetOrders());
		query = _splitQuery ? query.AsSplitQuery() : query;

		var resultQuery = _serviceProvider.GetRequiredService<TSelector>().Select(query);
		resultQuery = resultQuery.Union(union);

		if(resultOrder != null)
			resultQuery = GetOrderedQuery(resultQuery, resultOrder);

		var count = await PaginatedList<TResult>.CountAsync(resultQuery, cancellationToken);
		var result = (await PaginatedList<TResult>.CreateAsync(resultQuery, _filter.PageIndex, _pageSize, cancellationToken))
			.SetTotalCount(count);

		return Result<PaginatedList<TResult>>.Ok(result);
	}
	public async Task<Result<PaginatedList<TResult>>> ApplyWithUnionAsync<TResult>(
		IQueryable<TResult> union, 
		Expression<Func<TEntity, TResult>> selector,
		List<QueryableOrder> resultOrder = null,
		CancellationToken cancellationToken = default) 
		where TResult : class
	{
		if (_filter == null) throw new ArgumentNullException(nameof(_filter));

		if (_pageSize == 0) throw new ArgumentNullException(nameof(_pageSize));

		_sorter ??= _serviceProvider.GetRequiredService<ISorter<TEntity, TFilter>>();

		var query = _sorter.GetSort(_filter);
		query = GetOrderedQuery(query, _filter.GetOrders());

		var resultQuery = (_splitQuery ? query.AsSplitQuery() : query).Select(selector);
		resultQuery = resultQuery.Union(union);

		var count = await PaginatedList<TResult>.CountAsync(resultQuery, cancellationToken);

		if (resultOrder != null)
			resultQuery = GetOrderedQuery(resultQuery, resultOrder);

		var result = (await PaginatedList<TResult>.CreateAsync(resultQuery, _filter.PageIndex, _pageSize, cancellationToken))
			.SetTotalCount(count);

		return Result<PaginatedList<TResult>>.Ok(result);
	}

	private IQueryable<TOrderEntity> GetOrderedQuery<TOrderEntity>(IQueryable<TOrderEntity> source, List<QueryableOrder> orders)
		=> orders.Aggregate(source, (query, order) => query.NewOrder(order.PropertyName, order.OrderType));
}