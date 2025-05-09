using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Contents.Abstractions.Repositories;
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

	public override async Task<Result<FavoriteContent>> CreateAsync(FavoriteContent newEntity)
	{
		bool isDuplicate = await _entityRepository.ExistsByUserIdAndContentIdAsync(newEntity.UserId, newEntity.ContentId);

		if (isDuplicate)
			return Result<FavoriteContent>.Bad(FavoriteContentErrors.AlreadyFavorited(newEntity.ContentId));

		return await base.CreateAsync(newEntity);
	}

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

	public async Task<Result<FavoriteContent>> CreateAsync(string userId, int contentId)
	{
		bool isDuplicate = await _entityRepository.ExistsByUserIdAndContentIdAsync(userId, contentId);

		if (isDuplicate)
			return Result<FavoriteContent>.Bad(FavoriteContentErrors.AlreadyFavorited(contentId));

		var favoriteContent = new FavoriteContent
		{
			UserId = userId,
			ContentId = contentId
		};

		return await base.CreateAsync(favoriteContent);
	}
	public async Task<Result> DeleteAsync(string userId, int contentId)
	{
		var favoriteEntry = await _entityRepository.GetByUserIdAndContentIdAsync(userId, contentId);
		
		if (favoriteEntry == null)
			return Result.Ok();

		return await base.DeleteAsync(favoriteEntry);
	}
	public async Task<Result<bool>> IsFavoriteAsync(
		string userId, int contentId, CancellationToken cancellationToken = default)
	{
		bool isFavorite = await _entityRepository.ExistsByUserIdAndContentIdAsync(userId, contentId, cancellationToken);

		return Result<bool>.Ok(isFavorite);
	}
}