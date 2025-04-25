using Application.Results;
using Domain.Entities;

namespace Application.CinemaHalls;

/// <summary>
/// Defines error constants for cinema hall operations.
/// </summary>
public class CinemaHallErrors : EntityErrors<CinemaHall, int>
{
	/// <summary>
	/// Error indicating that the seats per row collection is empty or null.
	/// </summary>
	public static Error SeatsPerRowEmpty
		=> Error.BadRequest($"{EntityName}.{nameof(SeatsPerRowEmpty)}", "Seats per row cannot be empty or null.");

	/// <summary>
	/// Error indicating that the number of seats per row exceeds the maximum allowed limit (1000).
	/// </summary>
	public static Error SeatsPerRowTooMuch
		=> Error.BadRequest($"{EntityName}.{nameof(SeatsPerRowTooMuch)}", "Seats per row cannot be greater than 1000.");

	/// <summary>
	/// Error indicating that the seat row number is not greater than 0.
	/// </summary>
	public static Error SeatsRowEmpty
		=> Error.BadRequest($"{EntityName}.{nameof(SeatsRowEmpty)}", "Seat row must be greater than 0.");

	/// <summary>
	/// Error indicating that the seat row number exceeds the maximum allowed limit (1000).
	/// </summary>
	public static Error SeatsRowTooMuch
		=> Error.BadRequest($"{EntityName}.{nameof(SeatsRowTooMuch)}", "Seat row must be less than 1000.");
}