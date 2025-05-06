using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Contents.Requests;

/// <summary>
/// Represents the request model for creating new Content.
/// </summary>
public class CreateContentRequest
{
	/// <summary>
	/// Gets or sets the title of the content. This field is required and has a maximum length of 512 characters.
	/// </summary>
	/// <value>The title of the content (string).</value>
	[Required]
	[MaxLength(512)]
	public string Title { get; set; } = default!;

	/// <summary>
	/// Gets or sets the detailed description of the content. This field is required and has a maximum length of 16384 characters.
	/// </summary>
	/// <value>The detailed description of the content (string).</value>
	[Required]
	[MaxLength(16384)]
	public string Description { get; set; } = default!;

	/// <summary>
	/// Gets or sets the rating of the content. Maps to a nullable decimal (3, 1).
	/// </summary>
	/// <value>The rating of the content (decimal), nullable.</value>
	[Range(0.0, 100.0)]
	public decimal? Rating { get; set; }

	/// <summary>
	/// Gets or sets the release year of the content. This field is required.
	/// </summary>
	/// <value>The release year of the content (integer).</value>
	[Required]
	[Range(1800, 3000)]
	public int ReleaseYear { get; set; }

	/// <summary>
	/// Gets or sets the URL of the content's trailer. Maximum length is 2048 characters.
	/// </summary>
	/// <value>The URL of the content's trailer (string), nullable.</value>
	[MaxLength(2048)]
	public string? TrailerUrl { get; set; }

	/// <summary>
	/// Gets or sets the duration of the content in minutes. This field is required.
	/// </summary>
	/// <value>The duration of the content in minutes (integer).</value>
	[Required]
	[Range(1, int.MaxValue)]
	public int DurationMinutes { get; set; }

	/// <summary>
	/// Gets or sets the age rating for the content. This field is required.
	/// </summary>
	[Required]
	[Range(0, 100)]
	public int AgeRating { get; set; }

	/// <summary>
	/// Gets or sets the full name of the director. This field is required.
	/// </summary>
	[Required]
	[MaxLength(512)]
	public string DirectorFullName { get; set; } = default!;

	public List<int> GenreIds { get; set; } = [];
	public List<int> ActorIds { get; set; } = [];
}