using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Contents.Abstractions;
using Application.Genres.Abstractions;
using Application.Results;
using Domain.Entities;

namespace Application.Contents;

internal class ContentActorService(
	IContentService contentService,
	IGenreService genreService,
	IContentActorRepository entityRepository,
	IUnitOfWork unitOfWork) :
	BaseEntityService<ContentActor, int, IContentActorRepository>(entityRepository, unitOfWork), IContentActorService
{
	private readonly IContentService _contentService;
	private readonly IGenreService _genreService;

	protected override async Task<Result> ValidateEntityAsync(ContentActor entity)
	{
		if (Guard.MinLength(entity.RoleName, 1))
			return Result.Bad(EntityErrors<ContentActor, int>.StringTooShort(nameof(entity.RoleName), 1));

		if (Guard.MaxLength(entity.RoleName, 255))
			return Result.Bad(EntityErrors<ContentActor, int>.StringTooLong(nameof(entity.RoleName), 255));

		var contentExistsResult = await _contentService.VerifyExistsByIdAsync(entity.ContentId);

		if (contentExistsResult.IsFailure)
			return contentExistsResult.ToValue<ContentActor>();

		var genreExistsResult = await _genreService.VerifyExistsByIdAsync(entity.ActorId);

		if (genreExistsResult.IsFailure)
			return genreExistsResult.ToValue<ContentActor>();

		return Result.Ok();
	}
}