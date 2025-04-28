using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Bookings.Requests;

public class UpdateBookingStatusRequest
{
	/// <summary>
	/// Gets or sets the new status for the booking.
	/// </summary>
	[Required]
	public BookingStatus Status { get; set; }
}