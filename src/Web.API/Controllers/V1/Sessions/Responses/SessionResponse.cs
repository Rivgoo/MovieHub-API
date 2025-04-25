using Domain.Enums;

namespace Web.API.Controllers.V1.Sessions.Responses;

public class SessionResponse
{
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the start time of the session.
	/// </summary>
	public DateTime StartTime { get; set; }

	/// <summary>
	/// Gets or sets the unique identifier for the content.
	/// </summary>
	public int ContentId { get; set; }

	/// <summary>
	/// Gets or sets the cinema hall where the session is held.
	/// </summary>
	public int CinemaHallId { get; set; }

	/// <summary>
	/// Gets or sets the status of the session.
	/// </summary>
	public SessionStatus Status { get; set; }

	/// <summary>
	/// Gets or sets the ticket price for the session.
	/// </summary>
	public decimal TicketPrice { get; set; }
}