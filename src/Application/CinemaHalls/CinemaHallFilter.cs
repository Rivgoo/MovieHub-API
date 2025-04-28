using Application.Filters.Abstractions;
using Domain.Entities;

namespace Application.CinemaHalls;

public class CinemaHallFilter : BaseFilter<CinemaHall>
{
	/// <summary>
	/// Filter by name (case-insensitive, partial match).
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Filter by minimum number of rows (inclusive).
	/// </summary>
	public int? MinNumberOfRows { get; set; }

	/// <summary>
	/// Filter by maximum number of rows (inclusive).
	/// </summary>
	public int? MaxNumberOfRows { get; set; }
}