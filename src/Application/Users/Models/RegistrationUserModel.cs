namespace Application.Users.Models;

/// <summary>
/// Represents the data required to register a new user.
/// </summary>
/// <remarks>
/// This model is typically used as the input parameter for user registration operations,
/// containing the essential information provided by the user during signup.
/// </remarks>
public class RegistrationUserModel
{
	/// <summary>
	/// Gets or sets the first name of the user.
	/// </summary>
	public string FirstName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the last name of the user.
	/// </summary>
	public string LastName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the email address of the user.
	/// </summary>
	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the password for the user's account.
	/// </summary>
	public string Password { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the phone number of the user.
	/// </summary>
	public string PhoneNumber { get; set; } = string.Empty;
}