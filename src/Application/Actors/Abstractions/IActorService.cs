using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.Actors.Abstractions;

public interface IActorService : IEntityService<Actor, int>
{
}