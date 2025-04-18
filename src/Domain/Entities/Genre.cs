using Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Represents a genre of content, such as Action, Comedy, Drama, etc.
/// </summary>
public class Genre : BaseEntity<int>, IBaseEntity<int>
{
	/// <summary>
	/// Gets or sets the name for the genre.
	/// </summary>
	[Required]
	[MaxLength(512)]
	public string Name { get; set; } = default!;

	/// <summary>
	/// Gets or sets the collection of ContentGenre entries associated with this content.
	/// Represents the many-to-many relationship between Content and Genre (via ContentGenre).
	/// </summary>
	public ICollection<ContentGenre> ContentGenres { get; set; } = default!;

	/// <summary>
	/// Gets or sets the collection of Content entries associated with this genre.
	/// </summary>
	public ICollection<Content> Contents { get; set; } = default!;
}