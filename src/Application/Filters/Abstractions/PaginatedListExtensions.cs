namespace Application.Filters.Abstractions;

public static class PaginatedListExtensions
{
	public static PaginatedList<TResult> ToPaginatedListAsync<TResult>(this IEnumerable<TResult> source)
	{
		return PaginatedList<TResult>.FromList([.. source]);
	}
}