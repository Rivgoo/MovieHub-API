using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Contents.Abstractions;

public interface IContentGenreRepository : IEntityOperations<ContentGenre, int>
{
}