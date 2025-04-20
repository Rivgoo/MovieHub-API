using Application.Results;
using Domain.Entities;

namespace Application.CinemaHalls;

public class CinemaHallErrors : EntityErrors<CinemaHall, int>
{
	public static Error SeatsPerRowEmpty 
		=> Error.BadRequest($"{EntityName}.{nameof(SeatsPerRowEmpty)}", "Seats per row cannot be empty or null.");

	public static Error SeatsRowEmpty
		=> Error.BadRequest($"{EntityName}.{nameof(SeatsRowEmpty)}", "Seat row must be greater than 0.");
}