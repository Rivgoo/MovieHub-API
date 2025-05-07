using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Users.Requests;

public class UpdateUserRequest
{
	/// <summary>
	///	Gets or sets the first name of the user.
	/// </summary>
	[Required]
	[MaxLength(255)]
	public string FirstName { get; set; }

	/// <summary>
	/// Gets or sets the last name of the user.
	/// </summary>
	[Required]
	[MaxLength(255)]
	public string LastName { get; set; }

	[Required]
	[MaxLength(255)]
	[EmailAddress]
	public string Email { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the user is blocked.
	/// </summary>
	[Required]
	public bool IsBlocked { get; set; }
}