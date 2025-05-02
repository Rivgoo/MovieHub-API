using Application.Contents.Abstractions.Repositories;
using Application.Contents.Dtos;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class ContentRepository(CoreDbContext dbContext) :
	OperationsRepository<Content, int>(dbContext), IContentRepository
{
	public async Task<ICollection<ContentDto>> GetAllContentDtosAsync(CancellationToken cancellationToken)
	{
		return await _entities
			.Select(x => new ContentDto
			{
				Id = x.Id,
				Title = x.Title,
				Rating = x.Rating,
				ReleaseYear = x.ReleaseYear,
				Description = x.Description,
				PosterUrl = x.PosterUrl,
				TrailerUrl = x.TrailerUrl,
				BannerUrl = x.BannerUrl,
				DurationMinutes = x.DurationMinutes,
				CreatedAt = x.CreatedAt,
				UpdatedAt = x.UpdatedAt,
				GenreIds = x.ContentGenres.Select(g => g.Id).ToList(),
				ActorIds = x.ContentActors.Select(a => a.Id).ToList()
			})
			.ToListAsync(cancellationToken);
	}
	public async Task<ContentDto?> GetContentDtoAsync(int contentId, CancellationToken cancellationToken)
	{
		return await _entities
			.Where(x => x.Id == contentId)
			.Select(x => new ContentDto
			{
				Id = x.Id,
				Title = x.Title,
				Rating = x.Rating,
				ReleaseYear = x.ReleaseYear,
				Description = x.Description,
				PosterUrl = x.PosterUrl,
				TrailerUrl = x.TrailerUrl,
				BannerUrl = x.BannerUrl,
				DurationMinutes = x.DurationMinutes,
				CreatedAt = x.CreatedAt,
				UpdatedAt = x.UpdatedAt,
				GenreIds = x.ContentGenres.Select(g => g.Id).ToList(),
				ActorIds = x.ContentActors.Select(a => a.Id).ToList()
			})
			.FirstOrDefaultAsync(cancellationToken);
	}
}