using Application.Abstractions.Services;
using Application.Contents.Dtos;
using Application.Results;
using Domain.Entities;

namespace Application.Contents.Abstractions.Services;

public interface IContentService : IEntityService<Content, int>
{
	Task<Result<ContentDto>> GetContentDtoAsync(int contentId, CancellationToken cancellationToken = default);
	Task<ICollection<ContentDto>> GetAllContentDtosAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously saves a poster image for a content item from a Base64 string.
	/// </summary>
	/// <param name="contentId">The ID of the content item for which to save the poster.</param>
	/// <param name="base64String">The poster image content as a Base64 encoded string.</param>
	/// <returns>A Result containing the updated Content entity on success, or an Error on failure.</returns>
	Task<Result<Content>> SavePosterAsync(int contentId, string base64String);

	/// <summary>
	/// Asynchronously deletes the poster image for a content item.
	/// </summary>
	/// <param name="contentId">The ID of the content item for which to delete the poster.</param>
	/// <returns>A Result indicating success or failure.</returns>
	Task<Result> DeletePosterAsync(int contentId);

	/// <summary>
	/// Asynchronously saves a banner image for a content item from a Base64 string.
	/// </summary>
	/// <param name="contentId">The ID of the content item for which to save the banner.</param>
	/// <param name="base64String">The banner image content as a Base64 encoded string.</param>
	/// <returns>A Result containing the updated Content entity on success, or an Error on failure.</returns>
	Task<Result<Content>> SaveBannerAsync(int contentId, string base64String);

	/// <summary>
	/// Asynchronously deletes the banner image for a content item.
	/// </summary>
	/// <param name="contentId">The ID of the content item for which to delete the banner.</param>
	/// <returns>A Result indicating success or failure.</returns>
	Task<Result> DeleteBannerAsync(int contentId);
}