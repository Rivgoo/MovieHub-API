using Domain.Enums;

namespace Application.Bookings.Dtos;

/// <summary>
/// Data Transfer Object for Booking information.
/// </summary>
public class BookingDto
{
	/// <summary>
	/// Gets or sets the unique identifier for the booking.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the ID of the user who made the booking.
	/// </summary>
	public string UserId { get; set; } = default!;

	/// <summary>
	/// Gets or sets the ID of the session for this booking.
	/// </summary>
	public int SessionId { get; set; }

	/// <summary>
	/// Gets or sets the row number of the booked seat.
	/// </summary>
	public int RowNumber { get; set; }

	/// <summary>
	/// Gets or sets the seat number within the row.
	/// </summary>
	public int SeatNumber { get; set; }

	/// <summary>
	/// Gets or sets the status of the booking.
	/// </summary>
	public BookingStatus Status { get; set; }

	/// <summary>
	/// Gets or sets the creation timestamp.
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the last update timestamp.
	/// </summary>
	public DateTime UpdatedAt { get; set; }
}