using Application.Contents.Abstractions;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;

namespace Infrastructure.Repositories;

internal class FavoriteContentRepository(CoreDbContext dbContext) : 
	OperationsRepository<FavoriteContent, int>(dbContext), IFavoriteContentRepository
{
}