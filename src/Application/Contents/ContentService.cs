using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Contents.Abstractions;
using Application.Contents.Abstractions.Services;
using Application.Results;
using Application.Utilities;
using Domain.Entities;

namespace Application.Contents;

internal class ContentService(IContentRepository entityRepository, IUnitOfWork unitOfWork) : 
	BaseEntityService<Content, int, IContentRepository>(entityRepository, unitOfWork), IContentService
{
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

		if(entity.Rating != null && (Guard.Min(entity.Rating.Value, 0) || Guard.Max(entity.Rating.Value, 100)))
			return Result.Bad(ContentErrors.InvalidRating);

		if(entity.DurationMinutes <= 0)
			return Result.Bad(ContentErrors.InvalidDuration);

		if (entity.ReleaseYear <= 0)
			return Result.Bad(ContentErrors.InvalidReleaseYear);

		if(entity.TrailerUrl != null && !StringUtilities.IsValidWebUrlWithDomainCheck(entity.TrailerUrl))
			return Result.Bad(ContentErrors.InvalidTrailerUrl);

		return Result.Ok();
	}
}