using Application.Filters.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Application.Bookings;
public class BookingFilter : BaseFilter<Booking>
{
	/// <summary>
	/// Filter by the ID of the user who made the booking.
	/// </summary>
	public string? UserId { get; set; }

	/// <summary>
	/// Filter by the ID of the session.
	/// </summary>
	public int? SessionId { get; set; }

	/// <summary>
	/// Filter by booking status.
	/// </summary>
	public List<BookingStatus>? Statuses { get; set; }

	/// <summary>
	/// Filter by minimum creation date (inclusive).
	/// </summary>
	public DateTime? MinCreatedAt { get; set; }

	/// <summary>
	/// Filter by maximum creation date (inclusive).
	/// </summary>
	public DateTime? MaxCreatedAt { get; set; }

	/// <summary>
	/// Filter bookings associated with sessions for a specific Content ID.
	/// </summary>
	public int? ContentId { get; set; }

	/// <summary>
	/// Filter bookings associated with sessions in a specific Cinema Hall ID.
	/// </summary>
	public int? CinemaHallId { get; set; }
}