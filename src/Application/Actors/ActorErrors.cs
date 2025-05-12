using Application.Results;
using Domain.Entities;

namespace Application.Actors;

/// <summary>
/// Provides static definitions for common errors related to actor operations.
/// </summary>
public class ActorErrors : EntityErrors<Actor, int>
{
	/// <summary>
	/// Creates an error indicating that the actor's role was not found in the specified content.
	/// </summary>
	/// <param name="actorId">The ID of the actor.</param>
	/// <param name="contentId">The ID of the content.</param>
	/// <returns>An <see cref="Error"/> instance.</returns>
	public static Error RoleNotFoundInContent(int actorId, int contentId) => Error.NotFound(
		$"{EntityName}.RoleNotFoundInContent",
		$"Actor with ID {actorId} was not found playing a role in content ID {contentId}.");
}