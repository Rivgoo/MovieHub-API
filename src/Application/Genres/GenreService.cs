using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Genres.Abstractions;
using Application.Results;
using Application.Utilities;
using Domain.Entities;

namespace Application.Genres;

internal class GenreService(IGenreRepository entityRepository, IUnitOfWork unitOfWork) :
	BaseEntityService<Genre, int, IGenreRepository>(entityRepository, unitOfWork), IGenreService
{
	protected override async Task<Result> ValidateEntityAsync(Genre entity)
	{
		StringUtilities.TrimStringProperties(entity);

		if (Guard.MinLength(entity.Name, 1))
			return Result.Bad(EntityErrors<Genre, int>.StringTooShort(nameof(entity.Name), 1));

		if (Guard.MaxLength(entity.Name, 255))
			return Result.Bad(EntityErrors<Genre, int>.StringTooLong(nameof(entity.Name), 255));

		return Result.Ok();
	}
}