using Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Represents a many-to-many relationship between content and genres.
/// </summary>
public class ContentGenre : BaseEntity<int>, IBaseEntity<int>
{
	/// <summary>
	/// Gets or sets the unique identifier for the content-genre relationship.
	/// </summary>
	[Required]
	public int ContentId { get; set; }

	/// <summary>
	/// Gets or sets the unique identifier for the genre.
	/// </summary>
	[Required]
	public int GenreId { get; set; }

	/// <summary>
	/// Gets or sets the content associated with this relationship.
	/// </summary>
	public Content Content { get; set; } = default!;

	/// <summary>
	/// Gets or sets the genre associated with this relationship.
	/// </summary>
	public Genre Genre { get; set; } = default!;
}