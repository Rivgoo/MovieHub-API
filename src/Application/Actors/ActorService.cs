using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Actors.Abstractions;
using Application.Files;
using Application.Files.Abstractions;
using Application.Results;
using Application.Utilities;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Actors;

internal class ActorService(
	ILogger<ActorService> logger,
	IContentFileStorageService fileStorageService,
	IConfiguration configuration,
	IActorRepository entityRepository,
	IUnitOfWork unitOfWork) : 
	BaseEntityService<Actor, int, IActorRepository>(entityRepository, unitOfWork), IActorService
{
	private readonly ILogger<ActorService> _logger = logger;
	private readonly IContentFileStorageService _fileStorageService = fileStorageService;
	private readonly IConfiguration _configuration = configuration;

	public async Task<Result<Actor>> SavePhotoAsync(int actorId, string base64String)
	{
		var actorResult = await GetByIdAsync(actorId);

		if (actorResult.IsFailure)
			return actorResult;

		var actor = actorResult.Value!;

		if (!string.IsNullOrWhiteSpace(actor.PhotoUrl))
		{
			var deleteResult = await _fileStorageService.DeleteFileAsync(actor.PhotoUrl);

			if (deleteResult.IsFailure)
			{
				_logger.LogWarning("Failed to delete old photo file for actor {ActorId} at {PhotoUrl}. Error: {Error}", actorId, actor.PhotoUrl, deleteResult.Error?.Description);
				 return Result<Actor>.Bad(deleteResult.Error);
			}

			actor.PhotoUrl = null;
		}

		var actorPhotoPath = _configuration.GetValue<string>("ActorPhotoPath");
		if (string.IsNullOrWhiteSpace(actorPhotoPath))
			return Result<Actor>.Bad(FileErrors.InvalidDirectoryName("Actor photo path is not configured."));

		var saveResult = await _fileStorageService.SaveBase64ImageAsync(
			base64String,
			actorPhotoPath,
			actorId.ToString());

		if (saveResult.IsFailure)
			return Result<Actor>.Bad(saveResult.Error);

		actor.PhotoUrl = saveResult.Value;

		_entityRepository.Update(actor);
		await _unitOfWork.SaveChangesAsync();

		return Result<Actor>.Ok(actor);
	}

	public async Task<Result> DeletePhotoAsync(int actorId)
	{
		var actorResult = await GetByIdAsync(actorId);

		if (actorResult.IsFailure)
			return Result.Ok();

		var actor = actorResult.Value!;

		if (string.IsNullOrWhiteSpace(actor.PhotoUrl))
			return Result.Ok();

		var deleteResult = await _fileStorageService.DeleteFileAsync(actor.PhotoUrl);

		if (deleteResult.IsSuccess)
		{
			actor.PhotoUrl = null;

			_entityRepository.Update(actor);
			await _unitOfWork.SaveChangesAsync();

			return Result.Ok();
		}

		_logger.LogError("Failed to delete photo file for actor {ActorId} at {PhotoUrl}. Error: {Error}", actorId, actor.PhotoUrl, deleteResult.Error?.Description);
		
		return Result.Bad(deleteResult.Error ?? FileErrors.DeleteError("Unknown error deleting actor photo file."));
	}

	public override async Task<Result> DeleteAsync(Actor entity)
	{
		if (!string.IsNullOrEmpty(entity.PhotoUrl))
		{
			var photoDeletedResult = await _fileStorageService.DeleteFileAsync(entity.PhotoUrl);

			if (photoDeletedResult.IsFailure)
			{
				_logger.LogError($"Failed to delete photo file for actor {entity.Id} during entity deletion. Error: {photoDeletedResult.Error.Code}");

				 return Result.Bad(photoDeletedResult.Error);
			}
		}

		return await base.DeleteAsync(entity);
	}

	protected override async Task<Result> ValidateEntityAsync(Actor entity)
	{
		StringUtilities.TrimStringProperties(entity);

		if (Guard.MinLength(entity.FirstName, 1))
			return Result.Bad(EntityErrors<Actor, int>.StringTooShort(nameof(entity.FirstName), 1));

		if (Guard.MaxLength(entity.FirstName, 255))
			return Result.Bad(EntityErrors<Actor, int>.StringTooLong(nameof(entity.FirstName), 255));

		if (Guard.MinLength(entity.LastName, 1))
			return Result.Bad(EntityErrors<Actor, int>.StringTooShort(nameof(entity.LastName), 1));

		if (Guard.MaxLength(entity.LastName, 255))
			return Result.Bad(EntityErrors<Actor, int>.StringTooLong(nameof(entity.LastName), 255));

		return Result.Ok();
	}
}