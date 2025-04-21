using Application.Results;

namespace Application.Files.Abstractions;

public interface IContentFileStorageService
{
	/// <summary>
	/// Saves a file from a Base64 string to the specified public directory.
	/// </summary>
	/// <param name="base64String">The file content as a Base64 encoded string.</param>
	/// <param name="directoryName">The name of the subdirectory within the public storage root (e.g., "contents/posters").</param>
	/// <param name="fileNameWithoutExtension">The desired name for the file without extension (e.g., entity ID). A unique ID will be appended.</param>
	/// <returns>A Result containing the public relative URL path to the saved file on success, or an Error on failure.</returns>
	Task<Result<string>> SaveBase64ImageAsync(string base64String, string directoryName, string fileNameWithoutExtension);

	/// <summary>
	/// Deletes a file given its relative public URL path.
	/// </summary>
	/// <param name="relativeUrlPath">The relative URL path of the file to delete (e.g., "/contents/posters/1.jpeg").</param>
	/// <returns>A Result indicating success or failure (e.g., file not found).</returns>
	Task<Result> DeleteFileAsync(string relativeUrlPath);
}