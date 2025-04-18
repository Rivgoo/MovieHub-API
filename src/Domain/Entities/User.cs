using Domain.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Represents an application user within the identity system.
/// </summary>
public class User : IdentityUser, IBaseEntity<string>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="User"/> class.
	/// </summary>
	public User() : base() { }

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

	/// <summary>
	/// Gets or sets the first name of the user.
	/// </summary>
	[Required]
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the last name of the user.
	/// </summary>
	[Required]
	public DateTime UpdatedAt { get; set; }

	/// <summary>
	/// Gets or sets the date and time when the user last logged in.
	/// </summary>
	public DateTime? LastLoginAt { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the user is blocked.
	/// </summary>
	[Required]
	public bool IsBlocked { get; set; }
}