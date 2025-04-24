using Application.Results;

namespace Application.Filters.Abstractions;

public interface IFilter
{
	int PageIndex { get; set; }
	List<QueryableOrder> GetOrders();

	Result AddOrdering(QueryableOrderType orderType, string propertyName);
}