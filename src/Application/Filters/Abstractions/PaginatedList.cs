using Microsoft.EntityFrameworkCore;

namespace Application.Filters.Abstractions;

public class PaginatedList<TResult>
{
	public List<TResult> Items { get; private set; }

	public int PageIndex { get; private set; }
	public int PageSize { get; private set; }
	public int TotalCount { get; private set; }

	public int TotalPages => (int)Math.Ceiling(TotalCount / (float)PageSize);
	public bool HasPreviousPage => PageIndex > 1 && TotalPages > 1;
	public bool HasNextPage => PageIndex < TotalPages;

	public static PaginatedList<TResult> Empty => new([], 1, 1);

	public PaginatedList(List<TResult> items, int pageIndex = 1, int pageSize = 10)
	{
		if (pageSize <= 0)
			pageSize = 1;

		PageIndex = pageIndex;
		PageSize = pageSize;

		Items = items;
	}
	public PaginatedList<TResult> SetTotalCount(int totalCount)
	{
		TotalCount = totalCount;
		return this;
	}
	public PaginatedList<TResult> Clone(List<TResult> items)
	{
		return new PaginatedList<TResult>(items, PageIndex, PageSize)
		{
			TotalCount = TotalCount
		};
	}

	public static PaginatedList<TResult> FromList(List<TResult> items, int pageIndex = 1, int pageSize = 10)
	{
		return new PaginatedList<TResult>(items, pageIndex, pageSize);
	}

	public static async Task<int> CountAsync(IQueryable<TResult> source, CancellationToken cancellationToken = default)
		=> await source.CountAsync(cancellationToken);

	public static async Task<PaginatedList<TResult>> CreateAsync(
		IQueryable<TResult> source,
		int pageIndex,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		if(pageIndex < 1) pageIndex = 1;
		var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

		return new PaginatedList<TResult>(items, pageIndex, pageSize);
	}
}