using Domain.Abstractions;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Represents a session for a movie in a cinema hall.
/// </summary>
public class Session : BaseEntity<int>, IBaseEntity<int>
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
	/// Gets or sets the status of the session.
	/// </summary>
	[Required]
	public SessionStatus Status { get; set; }

	/// <summary>
	/// Gets or sets the ticket price for the session.
	/// </summary>
	[Required]
	[Column(TypeName = "decimal(18, 2)")]
	public decimal TicketPrice { get; set; }

	/// <summary>
	/// Gets or sets the content associated with the session.
	/// </summary>
	public Content Content { get; set; } = default!;

	/// <summary>
	/// Gets or sets the cinema hall associated with the session.
	/// </summary>
	public CinemaHall CinemaHall { get; set; } = default!;

	/// <summary>
	/// Gets or sets the collection of bookings associated with the session.
	/// </summary>
	public ICollection<Booking> Bookings { get; set; } = default!;
}