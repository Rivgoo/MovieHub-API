using Application.CinemaHalls.Abstractions;
using Application.CinemaHalls.Dtos;
using Domain.Entities;

namespace Infrastructure.Filters;

internal class CinemaHallSelector : ICinemaHallSelector
{
	public IQueryable<CinemaHallDto> Select(IQueryable<CinemaHall> source)
	{
		return source.Select(ch => new CinemaHallDto
		{
			Id = ch.Id,
			Name = ch.Name,
			SeatsPerRow = ch.SeatsPerRow,
			CreatedAt = ch.CreatedAt,
			UpdatedAt = ch.UpdatedAt
		});
	}
}