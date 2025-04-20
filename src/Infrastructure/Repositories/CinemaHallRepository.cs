using Application.CinemaHalls.Abstractions;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;

namespace Infrastructure.Repositories;

internal class CinemaHallRepository(CoreDbContext dbContext) : 
	OperationsRepository<CinemaHall, int>(dbContext), ICinemaHallRepository
{
}