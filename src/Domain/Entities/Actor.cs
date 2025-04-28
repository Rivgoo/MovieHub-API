using Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Represents an actor entity.
/// </summary>
public class Actor : BaseEntity<int>, IBaseEntity<int>
{
	/// <summary>
	/// Gets or sets the first name of the actor.
	/// </summary>
	[Required]
	[MaxLength(255)]
	public string FirstName { get; set; } = default!;

	/// <summary>
	/// Gets or sets the last name of the actor.
	/// </summary>
	[MaxLength(255)]
	public string LastName { get; set; } = default!;

	/// <summary>
	/// Gets or sets the relative URL path to the actor's photo.
	/// </summary>
	[MaxLength(512)]
	public string? PhotoUrl { get; set; }

	/// <summary>
	/// Gets or sets a collection of content actors associated with this actor.
	/// </summary>
	public ICollection<ContentActor> ContentActors { get; set; } = default!;

	/// <summary>
	/// Gets or sets a collection of contents associated with this actor.
	/// </summary>
	public ICollection<Content> Contents { get; set; } = default!;
}