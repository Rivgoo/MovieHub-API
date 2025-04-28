using Domain.Abstractions;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Represents a booking entity.
/// </summary>
public class Booking : BaseEntity<int>, IBaseEntity<int>
{
	/// <summary>
	/// Gets or sets the user ID associated with the booking.
	/// </summary>
	[Required]
	public string UserId { get; set; } = default!;

	/// <summary>
	/// Gets or sets the session ID associated with the booking.
	/// </summary>
	[Required]
	public int SessionId { get; set; }

	[Required]
	public int RowNumber { get; set; }

	[Required]
	public int SeatNumber { get; set; }

	/// <summary>
	/// Gets or sets the status of the booking.
	/// </summary>
	[Required]
	public BookingStatus Status { get; set; }

	/// <summary>
	/// Gets or sets the user associated with the booking.
	/// </summary>
	public User User { get; set; } = default!;

	/// <summary>
	/// Gets or sets the seat associated with the booking.
	/// </summary>
	public Session Session { get; set; } = default!;
}