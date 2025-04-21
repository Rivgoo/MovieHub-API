using Application.Abstractions.Services;
using Application.Results;
using Domain.Entities;

namespace Application.Contents.Abstractions.Services;

public interface IContentGenreService : IEntityService<ContentGenre, int>
{
	Task<Result<ContentGenre>> GetByDataAsync(int contentId, int genreId, CancellationToken cancellationToken = default);
}