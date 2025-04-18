using Application.Genres.Abstractions;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;

namespace Infrastructure.Repositories;

internal class GenreRepository(CoreDbContext dbContext) : 
	OperationsRepository<Genre>(dbContext), IGenreRepository
{
}