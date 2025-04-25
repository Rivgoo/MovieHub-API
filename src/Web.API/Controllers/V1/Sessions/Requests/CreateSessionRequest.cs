using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Sessions.Requests;

public class CreateSessionRequest
{
	/// <summary>
	/// Gets or sets the start time of the session.
	/// </summary>
	[Required]
	public DateTime StartTime { get; set; }

	/// <summary>
	/// Gets or sets the unique identifier for the content.
	/// </summary>
	[Required]
	public int ContentId { get; set; }

	/// <summary>
	/// Gets or sets the cinema hall where the session is held.
	/// </summary>
	[Required]
	public int CinemaHallId { get; set; }

	/// <summary>
	/// Gets or sets the ticket price for the session.
	/// </summary>
	[Required]
	public decimal TicketPrice { get; set; }
}