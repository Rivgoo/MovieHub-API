using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Bookings.Requests;

public class CreateBookingRequest
{
	/// <summary>
	/// Gets or sets the session ID associated with the booking.
	/// </summary>
	[Required]
	public int SessionId { get; set; }

	/// <summary>
	/// Gets or sets the row number of the booked seat.
	/// </summary>
	[Required]
	[Range(0, 1000)]
	public int RowNumber { get; set; }

	/// <summary>
	/// Gets or sets the seat number within the row.
	/// </summary>
	[Required]
	[Range(0, 1000)]
	public int SeatNumber { get; set; }
}