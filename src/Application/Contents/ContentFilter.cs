using Application.Filters.Abstractions;
using Domain.Entities;

namespace Application.Contents;

public class ContentFilter : BaseFilter<Content>
{
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

	/// <summary>
	/// Gets or sets a list of Actor IDs. Content must feature at least one (or all, see MatchAllActors) of these actors.
	/// </summary>
	public List<int> ActorIds { get; set; } = [];

	/// <summary>
	/// Gets or sets a flag indicating whether content must match ALL specified ActorIds (true)
	/// or just ANY (at least one) of the specified ActorIds (false).
	/// Defaults to false (ANY).
	/// </summary>
	public bool MatchAllActors { get; set; } = false;

	/// <summary>
	/// Gets or sets the User ID for whom to check favorite status.
	/// If set, filters content based on whether it's favorited by this user.
	/// Combine with <see cref="IsFavorited"/> flag.
	/// </summary>
	public string? FavoritedByUserId { get; set; }

	/// <summary>
	/// Gets or sets a flag indicating whether to retrieve content favorited by the specified user (true)
	/// or content *not* favorited by the specified user (false).
	/// Only effective if <see cref="FavoritedByUserId"/> is set.
	/// If <see cref="FavoritedByUserId"/> is set and this is null, the filter might be ignored or default to true depending on implementation.
	/// </summary>
	public bool? IsFavorited { get; set; }

	/// <summary>
	/// Gets or sets a flag to filter content based on whether it has any associated Sessions.
	/// true: only content with at least one Session.
	/// false: only content with no Sessions.
	/// null: ignore this filter.
	/// </summary>
	public bool? HasSessions { get; set; }

	/// <summary>
	/// Filter content that has at least one session starting on or after this date/time.
	/// </summary>
	public DateTime? MinSessionStartTime { get; set; }

	/// <summary>
	/// Filter content that has at least one session starting on or before this date/time.
	/// </summary>
	public DateTime? MaxSessionStartTime { get; set; }
}