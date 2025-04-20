using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Actors.Abstractions;

public interface IActorRepository : IEntityOperations<Actor, int>
{
}