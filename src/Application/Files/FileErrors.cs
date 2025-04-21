using Application.Results;

namespace Application.Files;

/// <summary>
/// Provides static definitions for errors related to file operations.
/// </summary>
/// <summary>
/// This class defines standard error objects for scenarios encountered
/// during file saving, deletion, or validation.
/// </summary>
public static class FileErrors
{
	private const string _filePrefix = "File.";

	/// <summary>
	/// Gets an error indicating that the provided Base64 string is invalid or malformed.
	/// </summary>
	/// <param name="description">Detailed description of why the Base64 is invalid.</param>
	/// <value>An Error instance for invalid Base64 input.</value>
	public static Error InvalidBase64(string description = "Invalid Base64 string.") =>
		Error.BadRequest($"{_filePrefix}InvalidBase64", description);

	/// <summary>
	/// Gets an error indicating that the provided image format is unsupported.
	/// </summary>
	/// <param name="description">Detailed description.</param>
	/// <value>An Error instance for an unsupported image format.</value>
	public static Error UnsupportedFormat(string description = "Unsupported image format.") =>
		Error.BadRequest($"{_filePrefix}UnsupportedFormat", description);

	/// <summary>
	/// Gets an error indicating an invalid directory name was provided.
	/// </summary>
	/// <value>An Error instance for an invalid directory name.</value>
	public static Error InvalidDirectoryName(string description = "Invalid directory name.") =>
		Error.BadRequest($"{_filePrefix}InvalidDirectoryName", description);

	/// <summary>
	/// Gets an error indicating an invalid file name was provided.
	/// </summary>
	/// <value>An Error instance for an invalid file name.</value>
	public static Error InvalidFileName(string description = "Invalid file name.") =>
		Error.BadRequest($"{_filePrefix}InvalidFileName", description);

	/// <summary>
	/// Gets an error indicating a failure during the file saving process.
	/// </summary>
	/// <value>An Error instance for a file save error.</value>
	public static Error SaveError(string description = "Failed to save file.") =>
		Error.InternalServerError($"{_filePrefix}SaveError", description);

	/// <summary>
	/// Gets an error indicating a failure during the file deletion process.
	/// </summary>
	/// <value>An Error instance for a file deletion error.</value>
	public static Error DeleteError(string description = "Failed to delete file.") =>
		Error.InternalServerError($"{_filePrefix}DeleteError", description);

	/// <summary>
	/// Gets an error indicating that a file was not found when attempting to delete it.
	/// </summary>
	/// <value>An Error instance for a file not found during deletion.</value>
	public static Error NotFound(string description = "File not found.") =>
		Error.NotFound($"{_filePrefix}NotFound", description);

	/// <summary>
	/// Gets an error indicating an attempt to access or delete a file outside the allowed directory.
	/// </summary>
	/// <value>An Error instance for an access denied error during file operation.</value>
	public static Error AccessDenied(string description = "Access denied to file.") =>
		Error.AccessForbidden($"{_filePrefix}AccessDenied", description);

	public static Error FileTooLarge(string description = "File size exceeds the allowed limit.") =>
		Error.AccessForbidden($"{_filePrefix}FileTooLarge", description);
}