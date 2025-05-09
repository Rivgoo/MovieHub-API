using Application.Abstractions.Services;
using Application.Results;
using Domain.Entities;

namespace Application.Contents.Abstractions.Services;

public interface IFavoriteContentService : IEntityService<FavoriteContent, int>
{
	Task<Result<FavoriteContent>> CreateAsync(string userId, int contentId);
	Task<Result> DeleteAsync(string userId, int contentId);
	Task<Result<bool>> IsFavoriteAsync(string userId, int contentId, CancellationToken cancellationToken = default);
}