﻿using Application.Contents.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class ContentGenreRepository(CoreDbContext dbContext) :
	OperationsRepository<ContentGenre, int>(dbContext), IContentGenreRepository
{
	public async Task<bool> ExistByDataAsync(int contentId, int genreId, CancellationToken cancellationToken)
	{
		return await _dbContext.ContentGenres
			.AnyAsync(x => x.ContentId == contentId && x.GenreId == genreId, cancellationToken);
	}

	public async Task<ContentGenre?> GetByDataAsync(int contentId, int genreId, CancellationToken cancellationToken)
	{
		return await _dbContext.ContentGenres
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.ContentId == contentId && x.GenreId == genreId, cancellationToken);
	}
}