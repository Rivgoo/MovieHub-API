namespace Application.Users.Models;

/// <summary>
/// Represents the outcome of an authentication attempt, including user information and tokens if successful, and flags for failure reasons.
/// </summary>
/// <remarks>
/// This Data Transfer Object (DTO) encapsulates the result of a login or authentication process,
/// indicating whether it succeeded and providing details about the user and obtained tokens
/// on success, or specific failure reasons on failure.
/// </remarks>
public class AuthenticationResult
{
	/// <summary>
	/// Gets or sets the user information if the authentication was successful; otherwise, null.
	/// </summary>
	/// <value>The <see cref="UserAuthenticationInfo"/> object for the authenticated user, or null if authentication failed.</value>
	public UserAuthenticationInfo? User { get; set; } = default!;

	/// <summary>
	/// Gets or sets a value indicating whether the authentication attempt was successful.
	/// </summary>
	/// <value><see langword="true"/> if the authentication process completed successfully; otherwise, <see langword="false"/>.</value>
	public bool Succeeded { get; set; } = false;

	/// <summary>
	/// Gets or sets a value indicating whether the user attempting to authenticate is locked out due to excessive failed login attempts.
	/// </summary>
	/// <value><see langword="true"/> if the user is locked out; otherwise, <see langword="false"/>.</value>
	public bool IsLockedOut { get; set; } = false;

	/// <summary>
	/// Gets or sets a value indicating whether the user's email address is not confirmed, and email confirmation is required for authentication.
	/// </summary>
	/// <value><see langword="true"/> if email is not confirmed and required; otherwise, <see langword="false"/>.</value>
	public bool IsEmailNotConfirmed { get; set; } = false;

	/// <summary>
	/// Gets or sets a value indicating whether the provided login credentials (username/password) were invalid.
	/// </summary>
	/// <value><see langword="true"/> if the input credentials were incorrect; otherwise, <see langword="false"/>.</value>
	public bool IsInvalidCredentials { get; set; } = false;

	/// <summary>
	/// Gets or sets a value indicating whether the user is blocked by means other than lockout (e.g., manual administrative block).
	/// </summary>
	/// <value><see langword="true"/> if the user is blocked; otherwise, <see langword="false"/>.</value>
	public bool IsBlocked { get; set; } = false;
}