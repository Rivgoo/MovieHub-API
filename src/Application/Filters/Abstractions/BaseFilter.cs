using Application.Results;
using Domain.Abstractions;

namespace Application.Filters.Abstractions;

public abstract class BaseFilter<TEntity> : IFilter where TEntity : IEntity
{
	public int PageIndex { get; set; } = 1;

	private List<QueryableOrder> _orders = [];

	public Result AddOrdering(QueryableOrderType orderType, string propertyName)
	{
		if (typeof(TEntity).GetProperty(propertyName) == null)
			return Result.Bad(FilterErrors.InvalidOrderField(propertyName));

		if(_orders.Any(x => x.PropertyName == propertyName))
			return Result.Bad(FilterErrors.OrderFieldAlreadyExists(propertyName));

		if ((orderType == QueryableOrderType.ThenBy || orderType == QueryableOrderType.ThenByDescending) && _orders.Count == 0)
			return Result.Bad(FilterErrors.InvalidSortOrder);

		_orders.Add(new QueryableOrder(propertyName, orderType));

		return Result.Ok();
	}
	public List<QueryableOrder> GetOrders() => _orders;

	public static string ValidateSearchTerms(string? searchTerm) 
		=> string.IsNullOrEmpty(searchTerm) ? string.Empty : searchTerm.Trim().Replace(">", "&gt;");
	public static string ValidateSearchTerms(string? searchTerm, int minLength, int maxLength)
	{
		searchTerm = ValidateSearchTerms(searchTerm);

		if(searchTerm.Length < minLength)
			return string.Empty;

		if(searchTerm.Length > maxLength)
			return searchTerm[..maxLength];

		return searchTerm;
	}

	public static List<string> ValidateList(List<string> list)
		=> [.. list.Where(x => !string.IsNullOrEmpty(x))];
	public static List<int> ValidateList(List<int> list)
		=> [.. list.Where(x => x >= 0)];
}