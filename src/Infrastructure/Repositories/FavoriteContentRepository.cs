using Application.Contents.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class FavoriteContentRepository(CoreDbContext dbContext) :
	OperationsRepository<FavoriteContent, int>(dbContext), IFavoriteContentRepository
{
	public async Task<bool> ExistsByUserIdAndContentIdAsync(
		string userId, int contentId, CancellationToken cancellationToken = default)
	{
		return await _dbContext.FavoriteContents
			.AsNoTracking()
			.AnyAsync(x => x.UserId == userId && x.ContentId == contentId, cancellationToken);
	}

	public async Task<FavoriteContent?> GetByUserIdAndContentIdAsync(string userId, int contentId, CancellationToken cancellationToken = default)
	{
		return await _dbContext.FavoriteContents
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.UserId == userId && x.ContentId == contentId, cancellationToken);
	}
}