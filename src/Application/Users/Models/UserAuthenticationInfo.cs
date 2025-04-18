namespace Application.Users.Models;

/// <summary>
/// Represents essential user information required for authentication and token generation.
/// </summary>
/// <remarks>
/// This Data Transfer Object (DTO) is used to bundle minimal user details (ID and role(s))
/// needed by authentication services (like JWT generation) after a user has been validated.
/// It avoids exposing the full User entity object to the authentication logic directly.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="UserAuthenticationInfo"/> class.
/// </remarks>
/// <param name="userId">The unique identifier of the user.</param>
/// <param name="role">The primary role of the user.</param>
public class UserAuthenticationInfo(string userId, string role)
{
	/// <summary>
	/// Gets or sets the unique identifier of the user.
	/// </summary>
	/// <value>The user's unique ID.</value>
	public string Id { get; set; } = userId;

	/// <summary>
	/// Gets or sets the primary role of the user.
	/// </summary>
	/// <value>The main role assigned to the user (e.g., "Admin", "Customer").</value>
	public string Role { get; set; } = role;
}