using Application.Actors.Dtos;
using Application.Filters.Abstractions;
using Domain.Entities;

namespace Application.Actors.Abstractions;

public interface IActorSelector : ISelector<Actor, ActorDto>
{
}