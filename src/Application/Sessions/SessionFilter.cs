using Application.Filters.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Application.Sessions;

public class SessionFilter : BaseFilter<Session>
{
	/// <summary>
	/// Filter by minimum start time (inclusive).
	/// </summary>
	public DateTime? MinStartTime { get; set; }

	/// <summary>
	/// Filter by maximum start time (inclusive).
	/// </summary>
	public DateTime? MaxStartTime { get; set; }

	/// <summary>
	/// Filter by specific Content ID.
	/// </summary>
	public int? ContentId { get; set; }

	/// <summary>
	/// Filter by specific Cinema Hall ID.
	/// </summary>
	public int? CinemaHallId { get; set; }

	/// <summary>
	/// Filter by session status.
	/// </summary>
	public SessionStatus? Status { get; set; }

	/// <summary>
	/// Filter by minimum ticket price (inclusive).
	/// </summary>
	public decimal? MinTicketPrice { get; set; }

	/// <summary>
	/// Filter by maximum ticket price (inclusive).
	/// </summary>
	public decimal? MaxTicketPrice { get; set; }

	/// <summary>
	/// Filter sessions that have available seats (at least one seat not booked).
	/// If true, only returns sessions with available seats. If false or null, this filter is ignored.
	/// </summary>
	public bool? HasAvailableSeats { get; set; }
}