using Application.Results;
using Domain.Entities;

namespace Application.Contents;

public class FavoriteContentErrors : EntityErrors<FavoriteContent, int>
{
	public static Error AlreadyFavorited(int contentId) => Error.Conflict(
		$"{EntityName}.AlreadyFavorited",
		$"Content with ID '{contentId}' is already in favorites for this user.");

	public static Error NotFavorited(int contentId) => Error.NotFound(
		$"{EntityName}.NotFavorited",
		$"Content with ID '{contentId}' is not in favorites for this user.");
}