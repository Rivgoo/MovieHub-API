using Domain.Enums;

namespace Application.Sessions.Dtos;

public class SessionContentDto
{
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the start time of the session.
	/// </summary>
	public DateTime StartTime { get; set; }

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

	/// <summary>
	/// Gets or sets the unique identifier for the content.
	/// </summary>
	public int ContentId { get; set; }

	/// <summary>
	/// Gets or sets the title of the content.
	/// </summary>
	public string Title { get; set; } = default!;

	/// <summary>
	/// Gets or sets the detailed description of the content.
	/// </summary>
	public string Description { get; set; } = default!;

	/// <summary>
	/// Gets or sets the rating of the content.
	/// </summary>
	public decimal? Rating { get; set; }

	/// <summary>
	/// Gets or sets the release year of the content.
	/// </summary>
	public int ReleaseYear { get; set; }

	/// <summary>
	/// Gets or sets the URL of the content's trailer.
	/// </summary>
	public string? TrailerUrl { get; set; }

	/// <summary>
	/// Gets or sets the URL of the content's banner image.
	/// </summary>
	public string? BannerUrl { get; set; }

	/// <summary>
	/// Gets or sets the URL of the content's poster image.
	/// </summary>
	public string? PosterUrl { get; set; }

	/// <summary>
	/// Gets or sets the duration of the content in minutes.
	/// </summary>
	public int DurationMinutes { get; set; }

	public List<int> GenreIds { get; set; }
}