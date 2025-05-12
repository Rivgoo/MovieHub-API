using Application.Abstractions.Services;
using Application.Actors.Dtos;
using Application.Results;
using Domain.Entities;

namespace Application.Contents.Abstractions.Services;

public interface IContentActorService : IEntityService<ContentActor, int>
{
	Task<Result<ContentActor>> GetByDataAsync(int id, int actorId, CancellationToken cancellationToken = default);
	Task<Result<bool>> ExistsByDataAsync(int id, int actorId, CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously retrieves information about an actor within a specific content, including their role name.
	/// </summary>
	/// <param name="actorId">The ID of the actor.</param>
	/// <param name="contentId">The ID of the content.</param>
	/// <param name="cancellationToken">A CancellationToken.</param>
	/// <returns>A Result containing the ActorInContentDto on success, or an Error on failure.</returns>
	Task<Result<ActorInContentDto>> GetActorInContentAsync(int actorId, int contentId, CancellationToken cancellationToken = default);
}