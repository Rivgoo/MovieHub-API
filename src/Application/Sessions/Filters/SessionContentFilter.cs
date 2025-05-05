using Application.Filters.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Application.Sessions;

public class SessionContentFilter : BaseFilter<Session>
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

	/// <summary>
	/// Gets or sets search terms to match against fields like Title and Description.
	/// The specific fields searched depend on the implementation logic using this filter.
	/// </summary>
	public string? SearchTerms { get; set; }

	/// <summary>
	/// Gets or sets the minimum rating for filtering content.
	/// Content with a rating less than this value will be excluded.
	/// </summary>
	public decimal? MinRating { get; set; }

	/// <summary>
	/// Gets or sets the maximum rating for filtering content.
	/// Content with a rating greater than this value will be excluded.
	/// </summary>
	public decimal? MaxRating { get; set; }

	/// <summary>
	/// Gets or sets the minimum release year for filtering content.
	/// Content released before this year will be excluded.
	/// </summary>
	public int? MinReleaseYear { get; set; }

	/// <summary>
	/// Gets or sets the maximum release year for filtering content.
	/// Content released after this year will be excluded.
	/// </summary>
	public int? MaxReleaseYear { get; set; }

	/// <summary>
	/// Gets or sets the minimum duration in minutes for filtering content.
	/// </summary>
	public int? MinDurationMinutes { get; set; }

	/// <summary>
	/// Gets or sets the maximum duration in minutes for filtering content.
	/// </summary>
	public int? MaxDurationMinutes { get; set; }

	/// <summary>
	/// Gets or sets a flag to filter content based on the presence of a TrailerUrl.
	/// true: only content with a TrailerUrl.
	/// false: only content without a TrailerUrl.
	/// null: ignore this filter.
	/// </summary>
	public bool? HasTrailer { get; set; }

	/// <summary>
	/// Gets or sets a flag to filter content based on the presence of a PosterUrl.
	/// true: only content with a PosterUrl.
	/// false: only content without a PosterUrl.
	/// null: ignore this filter.
	/// </summary>
	public bool? HasPoster { get; set; }

	/// <summary>
	/// Gets or sets a flag to filter content based on the presence of a BannerUrl.
	/// true: only content with a BannerUrl.
	/// false: only content without a BannerUrl.
	/// null: ignore this filter.
	/// </summary>
	public bool? HasBanner { get; set; }

	/// <summary>
	/// Gets or sets a list of Genre IDs. Content must belong to at least one (or all, see MatchAllGenres) of these genres.
	/// </summary>
	public List<int> GenreIds { get; set; } = [];

	/// <summary>
	/// Gets or sets a flag indicating whether content must match ALL specified GenreIds (true)
	/// or just ANY (at least one) of the specified GenreIds (false).
	/// Defaults to false (ANY).
	/// </summary>
	public bool MatchAllGenres { get; set; } = false;
}