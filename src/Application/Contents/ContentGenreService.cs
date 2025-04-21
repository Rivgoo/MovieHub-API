using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Contents.Abstractions;
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

	public async Task<Result<ContentGenre>> GetByDataAsync(int contentId, int genreId, CancellationToken cancellationToken = default)
	{
		var contentGenre = await _entityRepository.GetByDataAsync(contentId, genreId, cancellationToken);

		if (contentGenre == null)
			return Result<ContentGenre>.Bad(EntityErrors<ContentGenre, int>.NotFound);

		return Result<ContentGenre>.Ok(contentGenre);
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