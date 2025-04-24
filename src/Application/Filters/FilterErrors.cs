using Application.Results;

namespace Application.Filters;

public class FilterErrors
{
	public static Error InvalidOrderField(string propertyName) =>
		Error.BadRequest($"Filter.{nameof(InvalidOrderField)}", $"The field '{propertyName}' is not a valid order field.");

	public static Error OrderFieldAlreadyExists(string propertyName) =>
		Error.BadRequest($"Filter.{nameof(OrderFieldAlreadyExists)}", $"The field '{propertyName}' is already added as an order field.");

	public static Error InvalidOrderInput =>
		Error.BadRequest($"Filter.{nameof(InvalidOrderInput)}", "The order input is incorrect.");

	public static Error InvalidSortOrder =>
		Error.BadRequest($"Filter.{nameof(InvalidSortOrder)}", "The sort order is invalid. You must add at least one order field before using ThenBy or ThenByDescending.");
}