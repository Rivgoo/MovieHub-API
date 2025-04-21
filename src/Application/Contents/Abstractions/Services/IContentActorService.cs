using Application.Abstractions.Services;
using Application.Results;
using Domain.Entities;

namespace Application.Contents.Abstractions.Services;

public interface IContentActorService : IEntityService<ContentActor, int>
{
	Task<Result<ContentActor>> GetByDataAsync(int id, int actorId, CancellationToken cancellationToken = default);
}