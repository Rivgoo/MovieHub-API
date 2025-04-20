using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.CinemaHalls.Abstractions;

public interface ICinemaHallService : IEntityService<CinemaHall, int>
{
}