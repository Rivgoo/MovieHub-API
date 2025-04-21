using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Contents.Abstractions;
using Application.Contents.Abstractions.Services;
using Application.Contents.Dtos;
using Application.Files.Abstractions;
using Application.Results;
using Application.Utilities;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Contents;

internal class ContentService(
	ILogger<ContentService> logger,
	IContentFileStorageService fileStorageService,
	IConfiguration configuration,
	IContentRepository entityRepository,
	IUnitOfWork unitOfWork) :
	BaseEntityService<Content, int, IContentRepository>(entityRepository, unitOfWork), IContentService
{
	private readonly IContentFileStorageService _fileStorageService = fileStorageService;
	private readonly ILogger<ContentService> _logger = logger;
	private readonly IConfiguration _configuration = configuration;

	public async Task<Result<ContentDto>> GetContentDtoAsync(int contentId, CancellationToken cancellationToken = default)
	{
		var content = await _entityRepository.GetContentDtoAsync(contentId, cancellationToken);

		if (content == null)
			return Result<ContentDto>.Bad(ContentErrors.NotFoundById(contentId));

		return Result<ContentDto>.Ok(content);
	}
	public async Task<ICollection<ContentDto>> GetAllContentDtosAsync(CancellationToken cancellationToken = default)
	{
		var contents = await _entityRepository.GetAllContentDtosAsync(cancellationToken);

		return contents;
	}

	public async Task<Result<Content>> SavePosterAsync(int contentId, string base64String)
	{
		var content = await _entityRepository.GetByIdAsync(contentId);

		if (content == null)
			return Result<Content>.Bad(ContentErrors.NotFoundById(contentId));

		if (!string.IsNullOrWhiteSpace(content.PosterUrl))
		{
			var deleteResult = await _fileStorageService.DeleteFileAsync(content.PosterUrl);

			if (deleteResult.IsFailure)
			{
				_logger.LogWarning("Failed to delete old poster file for content {ContentId} at {PosterUrl}. Error: {Error}", contentId, content.PosterUrl, deleteResult.Error.Description);
				return Result<Content>.Bad(deleteResult.Error);
			}
		}

		var contentPosterPath = _configuration.GetValue<string>("ContentPosterPath");

		if (string.IsNullOrWhiteSpace(contentPosterPath))
			return Result<Content>.Bad(ContentErrors.InvalidPosterPath);

		var saveResult = await _fileStorageService.SaveBase64ImageAsync(
			base64String,
			contentPosterPath,
			contentId.ToString());

		if (saveResult.IsFailure)
			return Result<Content>.Bad(saveResult.Error);

		content.PosterUrl = saveResult.Value;

		_entityRepository.Update(content);
		await _unitOfWork.SaveChangesAsync();

		return Result<Content>.Ok(content);
	}
	public async Task<Result> DeletePosterAsync(int contentId)
	{
		var content = await _entityRepository.GetByIdAsync(contentId);

		if (content == null)
			return Result.Ok();

		if (string.IsNullOrWhiteSpace(content.PosterUrl))
			return Result.Ok();

		var deleteResult = await _fileStorageService.DeleteFileAsync(content.PosterUrl);

		if (deleteResult.IsSuccess)
		{
			content.PosterUrl = null;

			_entityRepository.Update(content);
			await _unitOfWork.SaveChangesAsync();

			return Result.Ok();
		}

		return Result.Bad(deleteResult.Error!);
	}

	protected override async Task<Result> ValidateEntityAsync(Content entity)
	{
		StringUtilities.TrimStringProperties(entity);

		if (Guard.MinLength(entity.Title, 1))
			return Result.Bad(EntityErrors<Content, int>.StringTooShort(nameof(entity.Title), 1));

		if (Guard.MaxLength(entity.Title, 512))
			return Result.Bad(EntityErrors<Content, int>.StringTooLong(nameof(entity.Title), 512));

		if (Guard.MinLength(entity.Description, 1))
			return Result.Bad(EntityErrors<Content, int>.StringTooShort(nameof(entity.Title), 1));

		if (Guard.MaxLength(entity.Description, 512))
			return Result.Bad(EntityErrors<Content, int>.StringTooLong(nameof(entity.Title), 16384));

		if (entity.Rating != null && (Guard.Min(entity.Rating.Value, 0) || Guard.Max(entity.Rating.Value, 100)))
			return Result.Bad(ContentErrors.InvalidRating);

		if (entity.DurationMinutes <= 0)
			return Result.Bad(ContentErrors.InvalidDuration);

		if (entity.ReleaseYear <= 0)
			return Result.Bad(ContentErrors.InvalidReleaseYear);

		if (entity.TrailerUrl != null && !StringUtilities.IsValidWebUrlWithDomainCheck(entity.TrailerUrl))
			return Result.Bad(ContentErrors.InvalidTrailerUrl);

		return Result.Ok();
	}
}