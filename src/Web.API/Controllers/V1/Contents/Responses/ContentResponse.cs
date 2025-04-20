namespace Web.API.Controllers.V1.Contents.Responses;

/// <summary>
/// Represents the response model for retrieving Content details.
/// </summary>
public class ContentResponse
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
	/// Gets or sets the release year of the content.
	/// </summary>
	public int ReleaseYear { get; set; }

	/// <summary>
	/// Gets or sets the URL of the content's trailer.
	/// </summary>
	public string? TrailerUrl { get; set; }

	/// <summary>
	/// Gets or sets the duration of the content in minutes.
	/// </summary>
	public int DurationMinutes { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}