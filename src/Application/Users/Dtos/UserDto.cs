namespace Application.Users.Dtos;

/// <summary>
/// Data Transfer Object for User information, including roles.
/// </summary>
public class UserDto
{
	/// <summary>
	/// Gets or sets the unique identifier for the user.
	/// </summary>
	public string Id { get; set; } = default!;

	/// <summary>
	/// Gets or sets the user's first name.
	/// </summary>
	public string FirstName { get; set; } = default!;

	/// <summary>
	/// Gets or sets the user's last name.
	/// </summary>
	public string LastName { get; set; } = default!;

	/// <summary>
	/// Gets or sets the user's email address.
	/// </summary>
	public string Email { get; set; } = default!;

	/// <summary>
	/// Gets or sets the user name.
	/// </summary>
	public string UserName { get; set; } = default!;

	/// <summary>
	/// Gets or sets the user's phone number.
	/// </summary>
	public string? PhoneNumber { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the user's email is confirmed.
	/// </summary>
	public bool EmailConfirmed { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the user account is blocked.
	/// </summary>
	public bool IsBlocked { get; set; }

	/// <summary>
	/// Gets or sets the creation timestamp.
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the last update timestamp.
	/// </summary>
	public DateTime UpdatedAt { get; set; }

	/// <summary>
	/// Gets or sets the last login timestamp (nullable).
	/// </summary>
	public DateTime? LastLoginAt { get; set; }

	/// <summary>
	/// Gets or sets the roles assigned to the user.
	/// </summary>
	public List<string> Roles { get; set; } = [];
}