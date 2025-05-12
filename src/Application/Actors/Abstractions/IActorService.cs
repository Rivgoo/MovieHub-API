using Application.Abstractions.Services;
using Application.Actors.Dtos;
using Application.Results;
using Domain.Entities;

namespace Application.Actors.Abstractions;

public interface IActorService : IEntityService<Actor, int>
{
	/// <summary>
	/// Asynchronously saves a photo for an actor from a Base64 string.
	/// </summary>
	/// <param name="actorId">The ID of the actor for whom to save the photo.</param>
	/// <param name="base64String">The photo content as a Base64 encoded string.</param>
	/// <returns>A Result containing the updated Actor entity on success, or an Error on failure.</returns>
	Task<Result<Actor>> SavePhotoAsync(int actorId, string base64String);

	/// <summary>
	/// Asynchronously deletes the photo for an actor.
	/// </summary>
	/// <param name="actorId">The ID of the actor for whom to delete the photo.</param>
	/// <returns>A Result indicating success or failure.</returns>
	Task<Result> DeletePhotoAsync(int actorId);
}