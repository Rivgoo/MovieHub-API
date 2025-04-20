using Application.Results;
using Domain.Entities;

namespace Application.Bookings;

/// <summary>
/// Provides static definitions for common errors related to booking operations.
/// </summary>
/// <summary>
/// This class inherits from <see cref="EntityErrors{TEntity, TId}"/> to provide a consistent naming convention
/// and structure for errors associated with the <see cref="Booking"/> entity.
/// These static properties and methods return <see cref="Error"/> instances
/// used to indicate specific failure reasons during booking creation or management.
/// </summary>
public class BookingErrors : EntityErrors<Booking, int>
{
	/// <summary>
	/// Gets an error indicating that the selected seat is already booked and unavailable.
	/// </summary>
	/// <value>An <see cref="Error"/> instance for the scenario where a seat is already booked.</value>
	public static Error SeatIsBooked => Error.AccessForbidden(
		$"{EntityName}.{nameof(SeatIsBooked)}", 
		"The selected seat is already booked.");

	public static Error SessionIsCompleted => Error.AccessForbidden(
		$"{EntityName}.{nameof(SessionIsCompleted)}",
		"The selected session has already completed.");

	public static Error SessionIsStarted => Error.AccessForbidden(
		$"{EntityName}.{nameof(SessionIsStarted)}",
		"The selected session has already started.");
}