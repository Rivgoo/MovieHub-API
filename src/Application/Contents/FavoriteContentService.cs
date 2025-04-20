using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Contents.Abstractions;
using Application.Contents.Abstractions.Services;
using Application.Results;
using Application.Users.Abstractions;
using Domain.Entities;

namespace Application.Contents;

internal class FavoriteContentService(
	IContentService contentService,
	IUserService userService,
	IFavoriteContentRepository entityRepository, 
	IUnitOfWork unitOfWork) : 
	BaseEntityService<FavoriteContent, int, IFavoriteContentRepository>(entityRepository, unitOfWork), IFavoriteContentService
{
	private readonly IContentService _contentService = contentService;
	private readonly IUserService _userService = userService;

	protected override async Task<Result> ValidateEntityAsync(FavoriteContent entity)
	{
		var contentExists = await _contentService.VerifyExistsByIdAsync(entity.ContentId);

		if (contentExists.IsFailure)
			return contentExists.ToValue<FavoriteContent>();

		var userExists = await _userService.VerifyExistsByIdAsync(entity.UserId);

		if (userExists.IsFailure)
			return userExists.ToValue<FavoriteContent>();

		return Result.Ok();
	}
}