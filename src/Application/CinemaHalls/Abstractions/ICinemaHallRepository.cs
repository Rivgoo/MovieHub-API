using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.CinemaHalls.Abstractions;

public interface ICinemaHallRepository : IEntityOperations<CinemaHall, int>
{
}