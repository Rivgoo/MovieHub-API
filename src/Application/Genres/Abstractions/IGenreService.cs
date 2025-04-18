using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.Genres.Abstractions;

public interface IGenreService : IEntityService<Genre, int>
{
}