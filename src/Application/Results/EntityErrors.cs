using Domain.Abstractions;

namespace Application.Results;

/// <summary>
/// Provides common error templates specific to a domain entity with a generic identifier.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity's unique identifier.</typeparam>
/// <remarks>
/// Constrained to work with types implementing <see cref="IBaseEntity{TId}"/>.
/// Assumes the entity type represents a database table or aggregate root.
/// </remarks>
public class EntityErrors<TEntity, TId>
	where TEntity : IBaseEntity<TId>, IEntity
	where TId : notnull, IComparable<TId>
{
	/// <summary>
	/// Gets the name of the entity type.
	/// </summary>
	public static string EntityName => typeof(TEntity).Name;

	/// <summary>
	/// Creates an error indicating that the entity with the specified identifier was not found.
	/// </summary>
	/// <param name="id">The identifier of the entity that was not found.</param>
	/// <returns>A <see cref="Error"/> instance representing the not found error.</returns>
	public static Error NotFoundById(TId id)
		=> Error.NotFound($"{EntityName}.NotFound", $"{EntityName} with Id '{id}' not found.");

	public static Error NotFound
		=> Error.NotFound($"{EntityName}.NotFound", $"{EntityName} not found.");

	/// <summary>
	/// Creates an error indicating that the entity object is null during a creation operation.
	/// </summary>
	/// <returns>A <see cref="Error"/> instance.</returns>
	public static Error CreateNullFailure
		=> Error.Failure($"{EntityName}.CreateNullFailure", $"Cannot create {EntityName}: the provided entity object is null.");

	/// <summary>
	/// Creates an error indicating that the entity object is null during an update operation.
	/// </summary>
	/// <returns>A <see cref="Error"/> instance.</returns>
	public static Error UpdateNullFailure
		=> Error.Failure($"{EntityName}.UpdateNullFailure", $"Cannot update {EntityName}: the provided entity object is null.");

	/// <summary>
	/// Creates an error indicating that the entity object is null during a delete operation.
	/// </summary>
	/// <returns>A <see cref="Error"/> instance.</returns>
	public static Error DeleteNullFailure
		=> Error.Failure($"{EntityName}.DeleteNullFailure", $"Cannot delete {EntityName}: the provided entity object is null.");

	/// <summary>
	/// Creates a validation error indicating a string property value is too long.
	/// </summary>
	/// <param name="propertyName">The name of the string property.</param>
	/// <param name="maxLength">The maximum allowed length.</param>
	/// <returns>A <see cref="Error"/> instance representing the validation error.</returns>
	public static Error StringTooLong(string propertyName, int maxLength)
		=> Error.BadRequest($"{EntityName}.Validation.{propertyName}TooLong", $"Property '{propertyName}' is too long. Maximum length is {maxLength}.");

	/// <summary>
	/// Creates a validation error indicating a string property value is too short, null, or empty.
	/// </summary>
	/// <param name="propertyName">The name of the string property.</param>
	/// <param name="minLength">The minimum required length.</param>
	/// <returns>A <see cref="Error"/> instance representing the validation error.</returns>
	public static Error StringTooShort(string propertyName, int minLength)
		=> Error.BadRequest($"{EntityName}.Validation.{propertyName}TooShort", $"Property '{propertyName}' must be at least {minLength} characters long.");

	/// <summary>
	/// Creates a validation error indicating a numeric or comparable property value is too high.
	/// </summary>
	/// <typeparam name="TValue">The type of the property value.</typeparam>
	/// <param name="propertyName">The name of the property.</param>
	/// <param name="maxValue">The maximum allowed value.</param>
	/// <returns>A <see cref="Error"/> instance representing the validation error.</returns>
	public static Error ValueTooHigh<TValue>(string propertyName, TValue maxValue)
		where TValue : IComparable<TValue>
		=> Error.BadRequest($"{EntityName}.Validation.{propertyName}TooHigh", $"Property '{propertyName}' must not be greater than {maxValue}.");

	/// <summary>
	/// Creates a validation error indicating a numeric or comparable property value is too low.
	/// </summary>
	/// <typeparam name="TValue">The type of the property value.</typeparam>
	/// <param name="propertyName">The name of the property.</param>
	/// <param name="minValue">The minimum required value.</param>
	/// <returns>A <see cref="Error"/> instance representing the validation error.</returns>
	public static Error ValueTooLow<TValue>(string propertyName, TValue minValue)
		where TValue : IComparable<TValue>
		=> Error.BadRequest($"{EntityName}.Validation.{propertyName}TooLow", $"Property '{propertyName}' must be at least {minValue}.");

	/// <summary>
	/// Creates a validation error indicating that a required property value is missing or null.
	/// </summary>
	/// <param name="propertyName">The name of the required property.</param>
	/// <returns>A <see cref="Error"/> instance representing the validation error.</returns>
	public static Error RequiredProperty(string propertyName)
		=> Error.BadRequest($"{EntityName}.Validation.{propertyName}Required", $"Property '{propertyName}' is required and cannot be null or empty.");

	/// <summary>
	/// Creates a validation error for an invalid format of a property value.
	/// </summary>
	/// <param name="propertyName">The name of the property with invalid format.</param>
	/// <param name="formatDescription">A description of the expected format.</param>
	/// <returns>A <see cref="Error"/> instance representing the validation error.</returns>
	public static Error InvalidFormat(string propertyName, string formatDescription = "invalid format")
		=> Error.BadRequest($"{EntityName}.Validation.{propertyName}InvalidFormat", $"Property '{propertyName}' has {formatDescription}.");

	/// <summary>
	/// Creates an error indicating a conflict, typically due to a unique constraint violation on a specific property.
	/// </summary>
	/// <typeparam name="TValue">The type of the conflicting value.</typeparam>
	/// <param name="propertyName">The name of the property that caused the conflict.</param>
	/// <param name="conflictingValue">The value of the property that caused the conflict.</param>
	/// <returns>A <see cref="Error"/> instance representing the conflict error.</returns>
	public static Error Conflict<TValue>(string propertyName, TValue conflictingValue)
	   where TValue : notnull
	   => Error.Conflict($"{EntityName}.Conflict.{propertyName}", $"{EntityName} with {propertyName} '{conflictingValue}' already exists.");

	/// <summary>
	/// Creates an error indicating that the entity is in an invalid state for the requested operation.
	/// </summary>
	/// <param name="currentState">A description of the entity's current state.</param>
	/// <param name="operationDescription">A description of the attempted operation.</param>
	/// <returns>A <see cref="Error"/> instance.</returns>
	public static Error InvalidState(string currentState, string operationDescription)
	   => Error.Failure($"{EntityName}.InvalidState", $"{EntityName} is in state '{currentState}' and cannot perform '{operationDescription}'.");
}