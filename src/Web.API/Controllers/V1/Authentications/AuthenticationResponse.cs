namespace Web.API.Controllers.V1.Authentications;

public class AuthenticationResponse
{
	/// <summary>
	/// Gets or sets the authentication token (e.g., JWT access token) if the authentication was successful; otherwise, an empty string.
	/// </summary>
	/// <value>The generated access token string if successful, or <see cref="string.Empty"/> on failure.</value>
	public string AccessToken { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the unique identifier of the user.
	/// </summary>
	/// <value>The user's unique ID.</value>
	public string UserId { get; set; }

	/// <summary>
	/// Gets or sets the primary role of the user.
	/// </summary>
	/// <value>The main role assigned to the user (e.g., "Admin", "Customer").</value>
	public string UserRole { get; set; }
}