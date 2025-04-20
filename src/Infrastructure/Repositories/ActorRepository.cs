using Application.Actors.Abstractions;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;

namespace Infrastructure.Repositories;

internal class ActorRepository(CoreDbContext dbContext) : 
	OperationsRepository<Actor, int>(dbContext), IActorRepository
{
}