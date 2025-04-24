using Application.Contents.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class ContentActorRepository(CoreDbContext dbContext) :
	OperationsRepository<ContentActor>(dbContext), IContentActorRepository
{
	public async Task<bool> ExistsByDataAsync(int id, int actorId, CancellationToken cancellationToken)
	{
		return await _entities
			.AnyAsync(x => x.ContentId == id && x.ActorId == actorId, cancellationToken);
	}

	public async Task<ContentActor?> GetByDataAsync(int id, int actorId, CancellationToken cancellationToken)
	{
		return await _entities.AsNoTracking()
			.Where(x => x.ContentId == id && x.ActorId == actorId)
			.FirstOrDefaultAsync(cancellationToken);
	}
}