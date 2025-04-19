using Application.Results;
using Domain.Entities;

namespace Application.Users;

/// <summary>
/// Provides static definitions for common errors related to user operations.
/// </summary>
/// <remarks>
/// This class inherits from <see cref="EntityErrors{TEntity, TId}"/> to provide a consistent naming convention
/// and structure for errors associated with the <see cref="User"/> entity.
/// These static properties and methods return <see cref="Error"/> instances
/// that can be used within application logic to indicate specific failure reasons
/// during user creation, update, authentication, or validation.
/// All errors defined here are of type <see cref="ErrorType.BadRequest"/>.
/// </remarks>
public class UserErrors : EntityErrors<User, string>
{
	/// <summary>
	/// Gets an error indicating that the provided email address format is invalid.
	/// </summary>
	/// <value>An <see cref="Error"/> instance for the invalid email scenario.</value>
	public static Error InvalidEmail
		=> Error.BadRequest($"{EntityName}.InvalidEmail", "The provided email address is invalid.");

	/// <summary>
	/// Gets an error indicating that the provided password does not meet the required criteria or format.
	/// </summary>
	/// <value>An <see cref="Error"/> instance for the invalid password scenario.</value>
	public static Error InvalidPassword
		=> Error.BadRequest($"{EntityName}.InvalidPassword", "The provided password is invalid.");

	/// <summary>
	/// Gets an error indicating that the provided phone number format is invalid.
	/// </summary>
	/// <value>An <see cref="Error"/> instance for the invalid phone number scenario.</value>
	public static Error InvalidPhoneNumber
		=> Error.BadRequest($"{EntityName}.InvalidPhoneNumber", "The provided phone number is invalid.");

	/// <summary>
	/// Creates an error indicating that a user with the specified email address already exists.
	/// </summary>
	/// <param name="email">The email address for which a user already exists.</param>
	/// <returns>An <see cref="Error"/> instance for the user already exists scenario, including the email.</returns>
	/// <remarks>
	/// This error should be used when an attempt is made to create a new user with an email
	/// address that is already registered in the system.
	/// </remarks>
	public static Error UserWithAlreadyExists(string? email)
		=> Error.Conflict($"{EntityName}.UserWithAlreadyExists", $"User with email '{email}' already exists.");
}