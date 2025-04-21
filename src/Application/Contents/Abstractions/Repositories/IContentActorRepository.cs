using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Contents.Abstractions;

public interface IContentActorRepository : IEntityOperations<ContentActor, int>
{
	Task<ContentActor?> GetByDataAsync(int id, int actorId, CancellationToken cancellationToken);
}