using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Contents.Requests;

/// <summary>
/// Represents the request model for updating existing Content.
/// </summary>
public class UpdateContentRequest
{
	/// <summary>
	/// Gets or sets the updated title of the content. This field is required and has a maximum length of 512 characters.
	/// </summary>
	/// <value>The updated title of the content (string).</value>
	[Required]
	[MaxLength(512)]
	public string Title { get; set; } = default!;

	/// <summary>
	/// Gets or sets the updated detailed description of the content. This field is required and has a maximum length of 16384 characters.
	/// </summary>
	/// <value>The updated detailed description of the content (string).</value>
	[Required]
	[MaxLength(16384)]
	public string Description { get; set; } = default!;

	/// <summary>
	/// Gets or sets the updated rating of the content. Maps to a nullable decimal (3, 1).
	/// </summary>
	/// <value>The updated rating of the content (decimal), nullable.</value>
	[Range(0.0, 100.0)]
	public decimal? Rating { get; set; }

	/// <summary>
	/// Gets or sets the updated release year of the content. This field is required.
	/// </summary>
	/// <value>The updated release year of the content (integer).</value>
	[Required]
	[Range(1800, 3000)]
	public int ReleaseYear { get; set; }

	/// <summary>
	/// Gets or sets the updated URL of the content's trailer. Maximum length is 2048 characters.
	/// </summary>
	/// <value>The updated URL of the content's trailer (string), nullable.</value>
	[MaxLength(2048)]
	public string? TrailerUrl { get; set; }

	/// <summary>
	/// Gets or sets the updated duration of the content in minutes. This field is required.
	/// </summary>
	/// <value>The updated duration of the content in minutes (integer).</value>
	[Required]
	[Range(1, int.MaxValue)]
	public int DurationMinutes { get; set; }
}