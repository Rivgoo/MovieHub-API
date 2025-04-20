using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.Contents.Abstractions.Services;

public interface IContentGenreService : IEntityService<ContentGenre, int>
{
}