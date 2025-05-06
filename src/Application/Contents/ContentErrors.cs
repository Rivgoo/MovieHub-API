using Application.Results;
using Domain.Entities;

namespace Application.Contents;
public class ContentErrors : EntityErrors<Content, int>
{
	public static Error InvalidRating => Error.BadRequest(
		$"{EntityName}.{nameof(InvalidRating)}",
		"Rating must be between 0 and 100 or null."
	);
	public static Error InvalidReleaseYear => Error.BadRequest(
		$"{EntityName}.{nameof(InvalidReleaseYear)}",
		"Release year must be a positive integer."
	);

	public static Error InvalidDuration => Error.BadRequest(
		$"{EntityName}.{nameof(InvalidDuration)}",
		"Duration must be a positive integer."
	);

	public static Error InvalidTrailerUrl => Error.BadRequest(
		$"{EntityName}.{nameof(InvalidTrailerUrl)}",
		"Trailer URL must be a valid URL."
	);

	public static Error InvalidPosterPath => Error.InternalServerError(
		$"{EntityName}.{nameof(InvalidPosterPath)}",
		"Poster path must be a valid URL."
	);
	public static Error InvalidAgeRating => Error.BadRequest(
		$"{EntityName}.{nameof(InvalidAgeRating)}",
		"Age rating must be between 0 and 100."
	);
}