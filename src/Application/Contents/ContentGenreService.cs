using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Contents.Abstractions.Repositories;
using Application.Contents.Abstractions.Services;
using Application.Genres.Abstractions;
using Application.Results;
using Domain.Entities;

namespace Application.Contents;

internal class ContentGenreService(
	IContentService contentService,
	IGenreService genreService,
	IContentGenreRepository entityRepository, 
	IUnitOfWork unitOfWork) : 
	BaseEntityService<ContentGenre, int, IContentGenreRepository>(entityRepository, unitOfWork), IContentGenreService
{
	private readonly IContentService _contentService = contentService;
	private readonly IGenreService _genreService = genreService;

	public async Task<Result<bool>> ExistsByDataAsync(int contentId, int genreId, CancellationToken cancellationToken = default)
	{
		var contentGenre = await _entityRepository.ExistByDataAsync(contentId, genreId, cancellationToken);

		if (contentGenre)
			return Result<bool>.Ok(true);

		return Result<bool>.Ok(false);
	}

	public async Task<Result<ContentGenre>> GetByDataAsync(int contentId, int genreId, CancellationToken cancellationToken = default)
	{
		var contentGenre = await _entityRepository.GetByDataAsync(contentId, genreId, cancellationToken);

		if (contentGenre == null)
			return Result<ContentGenre>.Bad(EntityErrors<ContentGenre, int>.NotFound);

		return Result<ContentGenre>.Ok(contentGenre);
	}

	public override async Task<Result<ContentGenre>> CreateAsync(ContentGenre newEntity)
	{
		var alreadyExists = await ExistsByDataAsync(newEntity.ContentId, newEntity.GenreId);

		if (alreadyExists.IsSuccess && alreadyExists.Value)
			return Result<ContentGenre>.Bad(Error.BadRequest($"{nameof(ContentGenre)}.AlreadyExists", "This genre is already linked to this content."));

		return await base.CreateAsync(newEntity);
	}

	protected override async Task<Result> ValidateEntityAsync(ContentGenre entity)
	{
		var contentExists = await _contentService.VerifyExistsByIdAsync(entity.ContentId);

		if (contentExists.IsFailure)
			return contentExists;

		var genreExists = await _genreService.VerifyExistsByIdAsync(entity.GenreId);

		if (genreExists.IsFailure)
			return genreExists;

		return Result.Ok();
	}
}