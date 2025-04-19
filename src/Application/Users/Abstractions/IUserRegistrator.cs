using Application.Results;
using Application.Users.Models;
using Domain.Entities;

namespace Application.Users.Abstractions;

/// <summary>
/// Defines methods for registering new users with specific roles.
/// </summary>
/// <remarks>
/// This interface provides distinct methods for registering users based on their intended role,
/// such as customers or administrators. Implementations handle the underlying logic for
/// creating the user account, potentially assigning roles, and other related tasks like
/// hashing passwords or sending confirmation emails.
/// </remarks>
public interface IUserRegistrator
{
	/// <summary>
	/// Registers a new user with the 'Customer' role asynchronously.
	/// </summary>
	/// <param name="model">The data model containing the details of the user to register.</param>
	/// <returns>
	/// A <see cref="Task{TResult}"/> representing the asynchronous operation.
	/// The task result is a <see cref="Result{TValue}"/> where TValue is <see cref="User"/>.
	/// A successful result (<see cref="Result.IsSuccess"/> is <see langword="true"/>)
	/// contains the newly created <see cref="User"/> entity.
	/// A failed result (<see cref="Result.IsFailure"/> is <see langword="false"/>)
	/// contains an <see cref="Error"/> object detailing the reason for the failure
	/// (e.g., validation errors, user already exists, database error).
	/// </returns>
	Task<Result<User>> RegisterCustomerAsync(RegistrationUserModel model);

	/// <summary>
	/// Registers a new user with the 'Admin' role asynchronously.
	/// </summary>
	/// <param name="model">The data model containing the details of the user to register.</param>
	/// <returns>
	/// A <see cref="Task{TResult}"/> representing the asynchronous operation.
	/// The task result is a <see cref="Result{TValue}"/> where TValue is <see cref="User"/>.
	/// A successful result (<see cref="Result.IsSuccess"/> is <see langword="true"/>)
	/// contains the newly created <see cref="User"/> entity.
	/// A failed result (<see cref="Result.IsFailure"/> is <see langword="false"/>)
	/// contains an <see cref="Error"/> object detailing the reason for the failure
	/// (e.g., validation errors, user already exists, database error).
	/// </returns>
	Task<Result<User>> RegisterAdminAsync(RegistrationUserModel model);
}