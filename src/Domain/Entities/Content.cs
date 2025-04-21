using Domain.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Represents a piece of content, such as a movie or TV show.
/// </summary>
public class Content : BaseEntity<int>, IBaseEntity<int>
{
	/// <summary>
	/// Gets or sets the title of the content.
	/// </summary>
	[Required]
	[MaxLength(512)]
	public string Title { get; set; } = default!;

	/// <summary>
	/// Gets or sets the detailed description of the content.
	/// </summary>
	[Required]
	[MaxLength(16384)]
	public string Description { get; set; } = default!;

	/// <summary>
	/// Gets or sets the rating of the content.
	/// Maps to a nullable decimal column (precision 3, scale 1).
	/// </summary>
	[Column(TypeName = "decimal(3, 1)")]
	public decimal? Rating { get; set; }

	/// <summary>
	/// Gets or sets the release year of the content.
	/// </summary>
	[Required]
	public int ReleaseYear { get; set; }

	/// <summary>
	/// Gets or sets the URL of the content's trailer.
	/// </summary>
	[MaxLength(2048)]
	public string? TrailerUrl { get; set; }

	[MaxLength(512)]
	public string? PosterUrl { get; set; }

	/// <summary>
	/// Gets or sets the duration of the content in minutes.
	/// </summary>
	[Required]
	public int DurationMinutes { get; set; }

	/// <summary>
	/// Gets or sets the collection of FavoriteContent entries associated with this content.
	/// Represents the many-to-many relationship between Content and User (via FavoriteContent).
	/// </summary>
	public ICollection<FavoriteContent> FavoriteContents { get; set; } = default!;

	/// <summary>
	/// Gets or sets the collection of Sessions associated with this content.
	/// Represents the one-to-many relationship where a content can have multiple sessions.
	/// </summary>
	public ICollection<Session> Sessions { get; set; } = default!;

	/// <summary>
	/// Gets or sets the collection of ContentGenre entries associated with this content.
	/// Represents the many-to-many relationship between Content and Genre (via ContentGenre).
	/// </summary>
	public ICollection<ContentGenre> ContentGenres { get; set; } = default!;

	/// <summary>
	/// Gets or sets the collection of ContentActor entries associated with this content.
	/// Represents the many-to-many relationship between Content and Actor (via ContentActor).
	/// </summary>
	public ICollection<ContentActor> ContentActors { get; set; } = default!;
}