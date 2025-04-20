using Application.Contents.Abstractions;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;

namespace Infrastructure.Repositories;

internal class ContentGenreRepository(CoreDbContext dbContext) : 
	OperationsRepository<ContentGenre, int>(dbContext), IContentGenreRepository
{
}