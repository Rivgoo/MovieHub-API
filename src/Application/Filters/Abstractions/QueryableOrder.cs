using System.ComponentModel.DataAnnotations;

namespace Application.Filters.Abstractions;

/// <summary>
/// Represents an ordering directive for a queryable collection.
/// </summary>
/// <param name="propertyName">The name of the property to order by.</param>
/// <param name="orderType">The type of ordering to apply.</param>
public struct QueryableOrder(string propertyName, QueryableOrderType orderType)
{
	/// <summary>
	/// Gets or sets the name of the property to use for ordering.
	/// </summary>
	[Required]
	public string PropertyName { get; set; } = propertyName;

	/// <summary>
	/// Gets or sets the ordering type (OrderBy, OrderByDescending, ThenBy, ThenByDescending).
	/// </summary>
	[Required]
	public QueryableOrderType OrderType { get; set; } = orderType;
}