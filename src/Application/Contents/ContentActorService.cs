﻿using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Contents.Abstractions.Repositories;
using Application.Contents.Abstractions.Services;
using Application.Actors.Abstractions;
using Application.Results;
using Domain.Entities;
using Application.Actors.Dtos;
using Application.Actors;

namespace Application.Contents;

internal class ContentActorService(
	IContentService contentService,
	IActorService actorService,
	IContentActorRepository entityRepository,
	IUnitOfWork unitOfWork) :
	BaseEntityService<ContentActor, int, IContentActorRepository>(entityRepository, unitOfWork), IContentActorService
{
	private readonly IContentService _contentService = contentService;
	private readonly IActorService _actorService = actorService;

	public async Task<Result<ActorInContentDto>> GetActorInContentAsync(int actorId, int contentId, CancellationToken cancellationToken = default)
	{
		var actorResult = await _actorService.GetByIdAsync(actorId, cancellationToken);

		if (actorResult.IsFailure)
			return Result<ActorInContentDto>.Bad(actorResult.Error!);

		var actor = actorResult.Value!;

		var contentActor = await _entityRepository.GetByDataAsync(contentId, actorId, cancellationToken);

		if (contentActor == null)
			return Result<ActorInContentDto>.Bad(ActorErrors.RoleNotFoundInContent(actorId, contentId));

		var actorInContentDto = new ActorInContentDto
		{
			Id = actor.Id,
			FirstName = actor.FirstName,
			LastName = actor.LastName,
			PhotoUrl = actor.PhotoUrl,
			RoleName = contentActor.RoleName
		};

		return Result<ActorInContentDto>.Ok(actorInContentDto);
	}


	public async Task<Result<bool>> ExistsByDataAsync(int id, int actorId, CancellationToken cancellationToken = default)
	{
		var contentActor = await _entityRepository.ExistsByDataAsync(id, actorId, cancellationToken);

		return Result<bool>.Ok(contentActor);
	}

	public async Task<Result<ContentActor>> GetByDataAsync(int id, int actorId, CancellationToken cancellationToken = default)
	{
		var contentActor = await _entityRepository.GetByDataAsync(id, actorId, cancellationToken);

		if (contentActor == null)
			return Result<ContentActor>.Bad(EntityErrors<ContentActor, int>.NotFound);

		return Result<ContentActor>.Ok(contentActor);
	}

	public override async Task<Result<ContentActor>> CreateAsync(ContentActor newEntity)
	{
		var alreadyExists = await ExistsByDataAsync(newEntity.ContentId, newEntity.ActorId);

		if (alreadyExists.IsSuccess && alreadyExists.Value)
			return Result<ContentActor>.Bad(Error.BadRequest($"{nameof(ContentActor)}.AlreadyExists", "This actor is already linked to this content."));

		return await base.CreateAsync(newEntity);
	}

	protected override async Task<Result> ValidateEntityAsync(ContentActor entity)
	{
		if (Guard.MinLength(entity.RoleName, 1))
			return Result.Bad(EntityErrors<ContentActor, int>.StringTooShort(nameof(entity.RoleName), 1));

		if (Guard.MaxLength(entity.RoleName, 255))
			return Result.Bad(EntityErrors<ContentActor, int>.StringTooLong(nameof(entity.RoleName), 255));

		var contentExistsResult = await _contentService.VerifyExistsByIdAsync(entity.ContentId);

		if (contentExistsResult.IsFailure)
			return contentExistsResult;

		var actorExistsResult = await _actorService.VerifyExistsByIdAsync(entity.ActorId);

		if (actorExistsResult.IsFailure)
			return actorExistsResult;

		return Result.Ok();
	}
}