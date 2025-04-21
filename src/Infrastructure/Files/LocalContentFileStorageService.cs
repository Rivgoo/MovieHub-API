using Application.Files;
using Application.Files.Abstractions;
using Application.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Files;

public class LocalContentFileStorageService : IContentFileStorageService
{
	private readonly ILogger<LocalContentFileStorageService> _logger;
	private readonly IConfiguration _configuration;
	private readonly string _publicFilesRoot;

	public LocalContentFileStorageService(
		ILogger<LocalContentFileStorageService> logger, 
		IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;

		var publicDataFolder = configuration.GetValue<string>("PublicDataFolder");

		if (string.IsNullOrWhiteSpace(publicDataFolder))
			throw new ArgumentNullException(nameof(publicDataFolder), "Public data folder is not configured.");

		_publicFilesRoot = Path.Combine(AppContext.BaseDirectory, publicDataFolder);
	}

	public async Task<Result<string>> SaveBase64ImageAsync(string base64String, string directoryName, string fileNameWithoutExtension)
	{
		if (string.IsNullOrWhiteSpace(base64String))
			return Result<string>.Bad(FileErrors.InvalidBase64("Base64 string is null or empty."));

		if (string.IsNullOrWhiteSpace(directoryName))
			return Result<string>.Bad(FileErrors.InvalidDirectoryName("Directory name is null or empty."));

		if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
			return Result<string>.Bad(FileErrors.InvalidFileName("File name without extension is null or empty."));

		try
		{
			var bytes = Convert.FromBase64String(base64String);

			var maxFileSizeInBytes = _configuration.GetValue<int>("ContentPosterMaxSizeInKilobytes") * 1024;

			if(bytes.Length > maxFileSizeInBytes)
				return Result<string>.Bad(FileErrors.FileTooLarge($"File size exceeds the maximum limit of {maxFileSizeInBytes / 1024} KB."));

			string? fileExtension = GetImageExtension(bytes);

			if (fileExtension == null)
				return Result<string>.Bad(FileErrors.UnsupportedFormat("Unsupported image format. Support: JPG, JPEG, PNG, WEBP"));

			string cleanedDirectoryName = Path.Combine(directoryName.Split(Path.GetInvalidPathChars(), StringSplitOptions.RemoveEmptyEntries));
			string cleanedFileName = string.Join("_", fileNameWithoutExtension.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

			var relativeFolderPath = Path.Combine(cleanedDirectoryName);
			var absoluteFolderPath = Path.Combine(_publicFilesRoot, relativeFolderPath);

			var uniqueFileName = $"{cleanedFileName}_{Guid.NewGuid().ToString("N")}{fileExtension}";
			var absoluteFilePath = Path.Combine(absoluteFolderPath, uniqueFileName);
			var relativeUrlPath = $"/{relativeFolderPath.Replace('\\', '/')}/{uniqueFileName}";

			Directory.CreateDirectory(absoluteFolderPath);

			await File.WriteAllBytesAsync(absoluteFilePath, bytes);

			return Result<string>.Ok(relativeUrlPath);
		}
		catch (FormatException)
		{
			return Result<string>.Bad(FileErrors.InvalidBase64("Input string is not a valid Base64 string."));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to save base64 image.");
			return Result<string>.Bad(FileErrors.SaveError($"Failed to save file: {ex.Message}"));
		}
	}

	public async Task<Result> DeleteFileAsync(string relativeUrlPath)
	{
		if (string.IsNullOrWhiteSpace(relativeUrlPath) || !relativeUrlPath.StartsWith("/"))
			return Result.Ok();

		try
		{
			var absoluteFilePath = Path.Combine(_publicFilesRoot, relativeUrlPath.TrimStart('/'));

			var normalizedRoot = Path.GetFullPath(_publicFilesRoot + Path.DirectorySeparatorChar);
			var normalizedFilePath = Path.GetFullPath(absoluteFilePath);

			if (!normalizedFilePath.StartsWith(normalizedRoot))
			{
				_logger.LogWarning("Attempted to delete file outside public root: {RelativeUrlPath}", relativeUrlPath);
				return Result.Bad(FileErrors.AccessDenied("Attempted to delete file outside allowed directory."));
			}

			if (File.Exists(absoluteFilePath))
				await Task.Run(() => File.Delete(absoluteFilePath));

			return Result.Ok();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to delete file: {RelativeUrlPath}", relativeUrlPath);
			return Result.Bad(FileErrors.DeleteError($"Failed to delete file: {ex.Message}"));
		}
	}

	private static string GetImageExtension(byte[] bytes)
	{
		if (bytes.Length > 4)
		{
			if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
				return ".jpg";
			else if (bytes[0] == 0xff && bytes[1] == 0xd8 && bytes[2] == 0xff && bytes[3] == 0xe2 || bytes[3] == 0xe1 || bytes[3] == 0xe0 || bytes[3] == 0xdb)
				return ".jpeg";
			else if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
				return ".png";
			else if (bytes.Length > 12 && bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 && bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
				return ".webp";
		}

		return string.Empty;
	}
}