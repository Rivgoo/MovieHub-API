using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Actors.Abstractions;
using Application.Results;
using Application.Utilities;
using Domain.Entities;

namespace Application.Actors;

internal class ActorService(IActorRepository entityRepository, IUnitOfWork unitOfWork) : 
	BaseEntityService<Actor, int, IActorRepository>(entityRepository, unitOfWork), IActorService
{
	protected override async Task<Result> ValidateEntityAsync(Actor entity)
	{
		StringUtilities.TrimStringProperties(entity);

		if (Guard.MinLength(entity.FirstName, 1))
			return Result.Bad(EntityErrors<Actor, int>.StringTooShort(nameof(entity.FirstName), 1));

		if (Guard.MaxLength(entity.FirstName, 255))
			return Result.Bad(EntityErrors<Actor, int>.StringTooLong(nameof(entity.FirstName), 255));

		if (Guard.MinLength(entity.LastName, 1))
			return Result.Bad(EntityErrors<Actor, int>.StringTooShort(nameof(entity.LastName), 1));

		if (Guard.MaxLength(entity.LastName, 255))
			return Result.Bad(EntityErrors<Actor, int>.StringTooLong(nameof(entity.LastName), 255));

		return Result.Ok();
	}
}