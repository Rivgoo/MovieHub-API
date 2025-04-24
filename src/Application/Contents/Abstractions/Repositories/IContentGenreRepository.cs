using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Contents.Abstractions.Repositories;

public interface IContentGenreRepository : IEntityOperations<ContentGenre, int>
{
	Task<bool> ExistByDataAsync(int contentId, int genreId, CancellationToken cancellationToken);
	Task<ContentGenre?> GetByDataAsync(int contentId, int genreId, CancellationToken cancellationToken);
}