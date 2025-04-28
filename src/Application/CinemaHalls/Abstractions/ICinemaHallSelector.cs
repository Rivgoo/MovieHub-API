using Application.CinemaHalls.Dtos;
using Application.Filters.Abstractions;
using Domain.Entities;

namespace Application.CinemaHalls.Abstractions;

/// <summary>
/// Defines the contract for a selector that projects CinemaHall entities to CinemaHallDto objects.
/// </summary>
public interface ICinemaHallSelector : ISelector<CinemaHall, CinemaHallDto>
{
}