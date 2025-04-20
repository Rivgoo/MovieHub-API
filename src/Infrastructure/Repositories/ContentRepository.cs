using Application.Contents.Abstractions;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;

namespace Infrastructure.Repositories;

internal class ContentRepository(CoreDbContext dbContext) : 
	OperationsRepository<Content, int>(dbContext), IContentRepository
{
}