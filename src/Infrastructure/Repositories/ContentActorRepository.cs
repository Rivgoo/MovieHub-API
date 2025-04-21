using Application.Contents.Abstractions;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class ContentActorRepository(CoreDbContext dbContext) :
	OperationsRepository<ContentActor>(dbContext), IContentActorRepository
{
	public async Task<ContentActor?> GetByDataAsync(int id, int actorId, CancellationToken cancellationToken)
	{
		return await _entities.AsNoTracking()
			.Where(x => x.ContentId == id && x.ActorId == actorId)
			.FirstOrDefaultAsync(cancellationToken);
	}
}