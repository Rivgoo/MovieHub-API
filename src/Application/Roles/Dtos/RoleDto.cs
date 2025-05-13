namespace Application.Roles.Dtos;

/// <summary>
/// Data Transfer Object for Role information.
/// </summary>
public class RoleDto
{
	/// <summary>
	/// Gets or sets the unique identifier for the role.
	/// </summary>
	public string Id { get; set; } = default!;

	/// <summary>
	/// Gets or sets the name of the role.
	/// </summary>
	public string Name { get; set; } = default!;
}