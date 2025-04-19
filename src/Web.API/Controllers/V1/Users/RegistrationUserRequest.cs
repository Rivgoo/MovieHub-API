using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Users;

/// <summary>
/// Represents the request model for user registration via the API.
/// </summary>
public class RegistrationUserRequest
{
	/// <summary>
	/// Gets or sets the first name of the user. This field is required and has a maximum length of 255 characters.
	/// </summary>
	/// <value>The user's first name (string).</value>
	[Required]
	[MaxLength(255)]
	public string FirstName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the last name of the user. This field is required and has a maximum length of 255 characters.
	/// </summary>
	/// <value>The user's last name (string).</value>
	[Required]
	[MaxLength(255)]
	public string LastName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the email address of the user. This field is required, must be a valid email format, and has a maximum length of 255 characters.
	/// </summary>
	/// <value>The user's email address (string), used as a unique identifier.</value>
	[Required]
	[EmailAddress]
	[MaxLength(255)]
	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the password for the user's account. This field is required.
	/// </summary>
	/// <value>The user's chosen password (string). Password strength validation is typically handled by the application service layer.</value>
	[Required]
	public string Password { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the phone number of the user. This field is required, must be in a valid phone number format, and has a maximum length of 16 characters.
	/// </summary>
	/// <value>The user's phone number (string).</value>
	[Required]
	[Phone]
	[MaxLength(16)]
	public string PhoneNumber { get; set; } = string.Empty;
}