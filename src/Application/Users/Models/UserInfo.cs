namespace Application.Users.Models;

/// <summary>
/// Represents basic information about a user.
/// </summary>
public class UserInfo
{
	/// <summary>
	/// Gets or sets the user's first name.
	/// </summary>
	public string FirstName { get; set; }

	/// <summary>
	/// Gets or sets the user's last name.
	/// </summary>
	public string LastName { get; set; }

	/// <summary>
	/// Gets or sets the user's email address.
	/// </summary>
	public string Email { get; set; }
}