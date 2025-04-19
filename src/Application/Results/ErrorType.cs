namespace Application.Results;

/// <summary>
/// Represents the types of errors that can occur within the application or system operations.
/// </summary>
/// <remarks>
/// These error types categorize the nature of a failure or problem,
/// aiding in consistent error handling and mapping (e.g., to HTTP status codes).
/// </remarks>
[Flags]
public enum ErrorType
{
	/// <summary>
	/// Indicates a general failure or an error that does not fit into more specific categories.
	/// </summary>
	Failure = 0,

	/// <summary>
	/// Indicates that a requested resource or entity could not be found.
	/// </summary>
	/// <remarks>Often maps to HTTP 404 Not Found.</remarks>
	NotFound = 1,

	/// <summary>
	/// Indicates that the input data or request is invalid according to defined rules or constraints.
	/// </summary>
	/// <remarks>Often maps to HTTP 400 Bad Request.</remarks>
	BadRequest = 2,

	/// <summary>
	/// Indicates a conflict with the current state of the resource or system,
	/// often due to a unique constraint violation or concurrency issue.
	/// </summary>
	/// <remarks>Often maps to HTTP 409 Conflict.</remarks>
	Conflict = 3,

	/// <summary>
	/// Indicates that the request requires user authentication, and the user is not authenticated.
	/// </summary>
	/// <remarks>Often maps to HTTP 401 Unauthorized.</remarks>
	AccessUnAuthorized = 4,

	/// <summary>
	/// Indicates that the user is authenticated but does not have the necessary permissions to perform the requested action.
	/// </summary>
	/// <remarks>Often maps to HTTP 403 Forbidden.</remarks>
	AccessForbidden = 5,

	/// <summary>
	/// Indicates an unexpected error occurred on the server side that prevented the request from being fulfilled.
	/// </summary>
	/// <remarks>Often maps to HTTP 500 Internal Server Error.</remarks>
	InternalServerError = 6
}