using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Contents.Abstractions.Repositories;

public interface IContentActorRepository : IEntityOperations<ContentActor, int>
{
	Task<bool> ExistsByDataAsync(int id, int actorId, CancellationToken cancellationToken);
	Task<ContentActor?> GetByDataAsync(int id, int actorId, CancellationToken cancellationToken);
}