using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.Contents.Abstractions;

public interface IContentGenreService : IEntityService<ContentGenre, int>
{
}