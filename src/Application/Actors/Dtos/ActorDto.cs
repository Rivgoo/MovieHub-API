namespace Application.Actors.Dtos;

/// <summary>
/// Data Transfer Object for Actor information.
/// </summary>
public class ActorDto
{
	/// <summary>
	/// Gets or sets the unique identifier for the actor.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the first name of the actor.
	/// </summary>
	public string FirstName { get; set; } = default!;

	/// <summary>
	/// Gets or sets the last name of the actor.
	/// </summary>
	public string LastName { get; set; } = default!;

	/// <summary>
	/// Gets or sets the relative URL path to the actor's photo.
	/// The full URL should be constructed by the client or presentation layer.
	/// </summary>
	public string? PhotoUrl { get; set; }

	/// <summary>
	/// Gets or sets the date and time the actor was created.
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the date and time the actor was last updated.
	/// </summary>
	public DateTime UpdatedAt { get; set; }
}