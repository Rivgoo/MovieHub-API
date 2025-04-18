using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Genres.Abstractions;

public interface IGenreRepository : IEntityOperations<Genre, int>
{
}