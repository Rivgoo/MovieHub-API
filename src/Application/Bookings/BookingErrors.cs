using Application.Results;
using Domain.Entities;

namespace Application.Bookings;

/// <summary>
/// Provides static definitions for common errors related to booking operations.
/// </summary>
public class BookingErrors : EntityErrors<Booking, int>
{
	/// <summary>
	/// Gets an error indicating that the selected seat is already booked and unavailable.
	/// </summary>
	public static Error SeatIsBooked => Error.Conflict(
		$"{EntityName}.{nameof(SeatIsBooked)}",
		"The selected seat is already booked.");

	/// <summary>
	/// Gets an error indicating that the session has already completed.
	/// </summary>
	public static Error SessionIsCompleted => Error.BadRequest(
		$"{EntityName}.{nameof(SessionIsCompleted)}",
		"The selected session has already completed and cannot be booked.");

	/// <summary>
	/// Gets an error indicating that the session has already started.
	/// </summary>
	public static Error SessionIsStarted => Error.BadRequest(
		$"{EntityName}.{nameof(SessionIsStarted)}",
		"The selected session has already started and cannot be booked.");

	/// <summary>
	/// Creates an error indicating an invalid row number for the cinema hall.
	/// </summary>
	/// <param name="row">The invalid row number provided.</param>
	/// <param name="maxRows">The maximum valid row number for the hall.</param>
	/// <returns>An <see cref="Error"/> instance.</returns>
	public static Error InvalidRowNumber(int row, int maxRows) => Error.BadRequest(
		$"{EntityName}.{nameof(InvalidRowNumber)}",
		$"Invalid row number '{row}'. Row must be between 1 and {maxRows}.");

	/// <summary>
	/// Creates an error indicating an invalid seat number for the specified row in the cinema hall.
	/// </summary>
	/// <param name="seat">The invalid seat number provided.</param>
	/// <param name="row">The row number for which the seat number is invalid.</param>
	/// <param name="maxSeats">The maximum valid seat number for the specified row.</param>
	/// <returns>An <see cref="Error"/> instance.</returns>
	public static Error InvalidSeatNumber(int seat, int row, int maxSeats) => Error.BadRequest(
		$"{EntityName}.{nameof(InvalidSeatNumber)}",
		$"Invalid seat number '{seat}' for row '{row}'. Seat must be between 1 and {maxSeats}.");

	public static Error UserIdMissing =>
		Error.BadRequest(
			$"{EntityName}.{nameof(UserIdMissing)}",
			"User ID could not be determined from token.");

	public static Error AccessDenied =>
		Error.AccessForbidden(
			$"{EntityName}.{nameof(AccessDenied)}",
			"User does not have permission to access this booking.");

	public static Error CannotCancelCompletedSession => Error.BadRequest(
		$"{EntityName}.{nameof(CannotCancelCompletedSession)}",
		"Cannot cancel a booking for a session that has already completed.");

	public static Error CannotCancelStartedSession => Error.BadRequest(
		$"{EntityName}.{nameof(CannotCancelStartedSession)}",
		"Cannot cancel a booking for a session that has already started or is ongoing.");
}