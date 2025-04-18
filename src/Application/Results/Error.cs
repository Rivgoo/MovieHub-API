using System.Net;

namespace Application.Results;

/// <summary>
/// Represents a specific error that occurred during an operation.
/// </summary>
/// <remarks>
/// This class provides details about a failure, including a unique code,
/// a human-readable description, and a categorized type.
/// Instances should be created using the static factory methods to ensure
/// correct association with an <see cref="ErrorType"/>.
/// </remarks>
public class Error
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Error"/> class with details about the error.
	/// </summary>
	/// <remarks>
	/// This constructor is private to enforce the use of static factory methods for creation,
	/// ensuring that the correct <see cref="ErrorType"/> is associated with the error.
	/// </remarks>
	/// <param name="code">The unique code identifying the specific error.</param>
	/// <param name="description">A human-readable description of the error.</param>
	/// <param name="errorType">The category or type of the error, as defined by <see cref="ErrorType"/>.</param>
	private Error(
		string code,
		string description,
		ErrorType errorType
	)
	{
		Code = code;
		Description = description;
		ErrorType = errorType;
	}

	/// <summary>
	/// Gets the unique code identifying the specific error.
	/// </summary>
	/// <value>A string representing the error code (e.g., "User.NotFound", "Validation.InvalidEmail").</value>
	public string Code { get; }

	/// <summary>
	/// Gets a human-readable description of the error.
	/// </summary>
	/// <value>A string providing more details about what went wrong (e.g., "User with Id '123' not found.").</value>
	public string Description { get; }

	/// <summary>
	/// Gets the category or type of the error.
	/// </summary>
	/// <value>An <see cref="ErrorType"/> value categorizing the nature of the error.</value>
	public ErrorType ErrorType { get; }

	/// <summary>
	/// Creates a new instance of <see cref="Error"/> representing a general failure.
	/// </summary>
	/// <param name="code">The unique error code.</param>
	/// <param name="description">A human-readable description of the failure.</param>
	/// <returns>A new instance of <see cref="Error"/> with <see cref="ErrorType"/> set to <see cref="ErrorType.Failure"/>.</returns>
	public static Error Failure(string code, string description) =>
		new(code, description, ErrorType.Failure);

	/// <summary>
	/// Creates a new instance of <see cref="Error"/> representing a 'not found' scenario.
	/// </summary>
	/// <param name="code">The unique error code.</param>
	/// <param name="description">A human-readable description indicating the resource was not found.</param>
	/// <returns>A new instance of <see cref="Error"/> with <see cref="ErrorType"/> set to <see cref="ErrorType.NotFound"/>.</returns>
	public static Error NotFound(string code, string description) =>
		new(code, description, ErrorType.NotFound);

	/// <summary>
	/// Creates a new instance of <see cref="Error"/> representing a validation error due to invalid input.
	/// </summary>
	/// <param name="code">The unique error code.</param>
	/// <param name="description">A human-readable description explaining the validation failure.</param>
	/// <returns>A new instance of <see cref="Error"/> with <see cref="ErrorType"/> set to <see cref="ErrorType.Validation"/>.</returns>
	public static Error Validation(string code, string description) =>
		new(code, description, ErrorType.Validation);

	/// <summary>
	/// Creates a new instance of <see cref="Error"/> representing a conflict error.
	/// </summary>
	/// <param name="code">The unique error code.</param>
	/// <param name="description">A human-readable description explaining the nature of the conflict (e.g., unique constraint violation).</param>
	/// <returns>A new instance of <see cref="Error"/> with <see cref="ErrorType"/> set to <see cref="ErrorType.Conflict"/>.</returns>
	public static Error Conflict(string code, string description) =>
		new(code, description, ErrorType.Conflict);

	/// <summary>
	/// Creates a new instance of <see cref="Error"/> representing an unauthorized access error.
	/// </summary>
	/// <param name="code">The unique error code.</param>
	/// <param name="description">A human-readable description indicating that authentication is required.</param>
	/// <returns>A new instance of <see cref="Error"/> with <see cref="ErrorType"/> set to <see cref="ErrorType.AccessUnAuthorized"/>.</returns>
	public static Error Unauthorized(string code, string description) =>
		new(code, description, ErrorType.AccessUnAuthorized);

	/// <summary>
	/// Creates a new instance of <see cref="Error"/> representing a forbidden access error.
	/// </summary>
	/// <param name="code">The unique error code.</param>
	/// <param name="description">A human-readable description indicating that the user lacks necessary permissions.</param>
	/// <returns>A new instance of <see cref="Error"/> with <see cref="ErrorType"/> set to <see cref="ErrorType.AccessForbidden"/>.</returns>
	public static Error AccessForbidden(string code, string description) =>
		new(code, description, ErrorType.AccessForbidden);

	 /// <summary>
	 /// Creates a new instance of <see cref="Error"/> representing an internal server error.
	 /// </summary>
	 public static Error InternalServerError(string code, string description) =>
		 new(code, description, ErrorType.InternalServerError);

	/// <summary>
	/// Maps an <see cref="ErrorType"/> to a corresponding <see cref="HttpStatusCode"/>.
	/// </summary>
	/// <param name="ErrorType">The <see cref="ErrorType"/> value to map. This parameter is extended.</param>
	/// <returns>The corresponding <see cref="HttpStatusCode"/>.</returns>
	/// <remarks>
	/// This mapping provides a standard way to translate internal error categories
	/// to external HTTP response statuses for API clients.
	/// </remarks>
	public HttpStatusCode ToHttpStatusCode()
	{
		return ErrorType switch
		{
			ErrorType.NotFound => HttpStatusCode.NotFound,
			ErrorType.Validation => HttpStatusCode.BadRequest,
			ErrorType.Conflict => HttpStatusCode.Conflict,
			ErrorType.AccessUnAuthorized => HttpStatusCode.Unauthorized,
			ErrorType.AccessForbidden => HttpStatusCode.Forbidden,
			ErrorType.Failure => HttpStatusCode.InternalServerError,
			ErrorType.InternalServerError => HttpStatusCode.InternalServerError,
			_ => HttpStatusCode.InternalServerError
		};
	}
}