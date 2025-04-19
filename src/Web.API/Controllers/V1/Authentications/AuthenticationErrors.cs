using Application.Results;

namespace Web.API.Controllers.V1.Authentications;

/// <summary>
/// Provides specific error templates for authentication-related scenarios.
/// </summary>
/// <remarks>
/// These static error definitions are used within the authentication process
/// to provide clear and consistent error codes and descriptions for various
/// authentication failure reasons. Each error is associated with a specific
/// <see cref="ErrorType"/> that helps in categorizing and handling the error.
/// </remarks>
public static class AuthenticationErrors
{
	private const string _authenticationPrefix = "Authentication.";

	/// <summary>
	/// Gets an error indicating that API authentication is globally disabled via configuration.
	/// </summary>
	/// <value>An <see cref="Error"/> instance for the API disabled scenario.</value>
	public static Error APIDisabled =>
		Error.AccessForbidden($"{_authenticationPrefix}APIDisabled", "API authentication is disabled.");

	/// <summary>
	/// Gets an error indicating that the provided login credentials (email/username and password) are invalid.
	/// </summary>
	/// <value>An <see cref="Error"/> instance for invalid credentials.</value>
	public static Error InvalidCredentials =>
		Error.Unauthorized($"{_authenticationPrefix}InvalidCredentials", "Invalid credentials.");

	/// <summary>
	/// Gets an error indicating that the user account is locked out due to excessive failed login attempts.
	/// </summary>
	/// <value>An <see cref="Error"/> instance for a locked out user.</value>
	public static Error UserLockedOut =>
		Error.AccessForbidden($"{_authenticationPrefix}UserLockedOut", "User account is locked out.");

	/// <summary>
	/// Gets an error indicating that the user's email address is not confirmed, and email confirmation is required for authentication.
	/// </summary>
	/// <value>An <see cref="Error"/> instance for an unconfirmed email scenario.</value>
	public static Error UserEmailNotConfirmed =>
		Error.AccessForbidden($"{_authenticationPrefix}UserEmailNotConfirmed", "User email is not confirmed.");

	/// <summary>
	/// Gets an error indicating that the user account is blocked by means other than lockout (e.g., manual administrative block).
	/// </summary>
	/// <value>An <see cref="Error"/> instance for a blocked user.</value>
	public static Error UserBlocked =>
		Error.AccessForbidden($"{_authenticationPrefix}UserBlocked", "User account is blocked.");
}