namespace Application.Actors.Dtos;

/// <summary>
/// Data Transfer Object for Actor information within a specific Content, including their role.
/// </summary>
public class ActorInContentDto
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
	/// </summary>
	public string? PhotoUrl { get; set; }

	/// <summary>
	/// Gets or sets the name of the role the actor played in the specific content.
	/// </summary>
	public string RoleName { get; set; } = default!;
}