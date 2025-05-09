using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Contents.Abstractions.Repositories;

public interface IFavoriteContentRepository : IEntityOperations<FavoriteContent, int>
{
	Task<FavoriteContent?> GetByUserIdAndContentIdAsync(string userId, int contentId, CancellationToken cancellationToken = default);
	Task<bool> ExistsByUserIdAndContentIdAsync(string userId, int contentId, CancellationToken cancellationToken = default);
}