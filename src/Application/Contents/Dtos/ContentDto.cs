namespace Application.Contents.Dtos;

public class ContentDto
{
	/// <summary>
	/// Gets or sets the unique identifier for the content.
	/// </summary>
	public int Id { get; set; }

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
	/// Gets or sets the age rating for the content.
	/// </summary>
	public int AgeRating { get; set; }

	/// <summary>
	/// Gets or sets the full name of the director.
	/// </summary>
	public string DirectorFullName { get; set; } = default!;

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
	public List<int> ActorIds { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}