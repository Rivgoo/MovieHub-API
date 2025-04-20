using Application.Abstractions;
using Application.Abstractions.Services;
using Application.CinemaHalls.Abstractions;
using Application.Results;
using Application.Utilities;
using Domain.Entities;

namespace Application.CinemaHalls;

internal class CinemaHallService(
	ICinemaHallRepository entityRepository, IUnitOfWork unitOfWork) : 
	BaseEntityService<CinemaHall, int, ICinemaHallRepository>(entityRepository, unitOfWork), ICinemaHallService
{
	protected override async Task<Result> ValidateEntityAsync(CinemaHall entity)
	{
		StringUtilities.TrimStringProperties(entity);

		if (Guard.MinLength(entity.Name, 1))
			return Result.Bad(EntityErrors<CinemaHall, int>.StringTooShort(nameof(entity.Name), 1));

		if (Guard.MaxLength(entity.Name, 512))
			return Result.Bad(EntityErrors<CinemaHall, int>.StringTooLong(nameof(entity.Name), 512));

		if(entity.SeatsPerRow == null || entity.SeatsPerRow.Count == 0)
			return Result.Bad(CinemaHallErrors.SeatsPerRowEmpty);

		foreach (var seats in entity.SeatsPerRow)
			if (seats <= 0)
				return Result.Bad(CinemaHallErrors.SeatsRowEmpty);

		return Result.Ok();
	}
}