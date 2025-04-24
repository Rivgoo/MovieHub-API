namespace Application.Filters.Abstractions;

[Flags]
public enum QueryableOrderType
{
	OrderBy,
	OrderByDescending,
	ThenBy,
	ThenByDescending
}