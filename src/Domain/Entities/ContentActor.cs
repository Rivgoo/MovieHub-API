using Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Represents a content actor entity.
/// </summary>
public class ContentActor : IEntity
{
	/// <summary>
	/// Gets or sets the unique identifier for the content actor.
	/// </summary>
	[Required]
	public int ContentId { get; set; }

	/// <summary>
	/// Gets or sets the unique identifier for the actor associated with the content.
	/// </summary>
	[Required]
	public int ActorId { get; set; }

	/// <summary>
	///	Gets or sets the role name of the actor in the content.
	/// </summary>
	[Required]
	[MaxLength(255)]
	public string RoleName { get; set; } = default!;

	/// <summary>
	/// Gets or sets the content associated with this content actor.
	/// </summary>
	public Content Content { get; set; } = default!;

	/// <summary>
	/// Gets or sets the actor associated with this content actor.
	/// </summary>
	public Actor Actor { get; set; } = default!;
}