using Application.Actors.Abstractions;
using Application.Actors.Dtos;
using Domain.Entities;

namespace Infrastructure.Filters;

internal class ActorSelector : IActorSelector
{
	public IQueryable<ActorDto> Select(IQueryable<Actor> source)
	{
		return source.Select(a => new ActorDto
		{
			Id = a.Id,
			FirstName = a.FirstName,
			LastName = a.LastName,
			PhotoUrl = a.PhotoUrl,
			CreatedAt = a.CreatedAt,
			UpdatedAt = a.UpdatedAt
		});
	}
}