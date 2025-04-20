using Application.Contents.Abstractions;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;

namespace Infrastructure.Repositories;

internal class ContentActorRepository(CoreDbContext dbContext) : 
	OperationsRepository<ContentActor>(dbContext), IContentActorRepository
{
}