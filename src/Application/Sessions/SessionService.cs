using Application.Abstractions;
using Application.Abstractions.Services;
using Application.CinemaHalls.Abstractions;
using Application.Contents.Abstractions.Services;
using Application.Results;
using Application.Sessions.Abstractions;
using Domain.Entities;

namespace Application.Sessions;

internal class SessionService(
	IContentService contentService,
	ICinemaHallService cinemaHallService,
	ISessionRepository entityRepository, 
	IUnitOfWork unitOfWork) : 
	BaseEntityService<Session, int, ISessionRepository>(entityRepository, unitOfWork), ISessionService
{
	private readonly IContentService _contentService = contentService;
	private readonly ICinemaHallService _cinemaHallService = cinemaHallService;

	protected override async Task<Result> ValidateEntityAsync(Session entity)
	{
		if(Guard.Min(entity.TicketPrice, 0) || Guard.Max(entity.TicketPrice, 100000))
			return Result.Bad(SessionErrors.InvalidTicketPrice);

		if(entity.StartTime < DateTime.UtcNow)
			return Result.Bad(SessionErrors.InvalidStartTime);

		var contentExistsResult = await _contentService.VerifyExistsByIdAsync(entity.ContentId);

		if (contentExistsResult.IsFailure)
			return contentExistsResult;

		var cinemaHallExistsResult = await _cinemaHallService.VerifyExistsByIdAsync(entity.CinemaHallId);

		if (cinemaHallExistsResult.IsFailure)
			return cinemaHallExistsResult;

		return Result.Ok();
	}
}