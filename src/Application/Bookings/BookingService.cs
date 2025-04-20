using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Bookings.Abstractions;
using Application.Results;
using Application.Sessions.Abstractions;
using Application.Users.Abstractions;
using Domain.Entities;

namespace Application.Bookings;

internal class BookingService(
	IUserService userService,
	ISessionService sessionService,
	IBookingRepository entityRepository, 
	IUnitOfWork unitOfWork) : 
	BaseEntityService<Booking, int, IBookingRepository>(entityRepository, unitOfWork), IBookingService
{
	private readonly IUserService _userService = userService;
	private readonly ISessionService _sessionService = sessionService;

	public async Task<bool> IsSeatBooked(int sessionId, int seatId)
		=> await _entityRepository.IsSeatBooked(sessionId, seatId);

	protected override async Task<Result> ValidateEntityAsync(Booking entity)
	{
		var sessionExists = await _sessionService.VerifyExistsByIdAsync(entity.SessionId);

		if (sessionExists.IsFailure)
			return sessionExists;

		var userExists = await _userService.VerifyExistsByIdAsync(entity.UserId);

		if (userExists.IsFailure)
			return userExists;

		if(await IsSeatBooked(entity.SessionId, entity.SeatId))
			return Result.Bad(BookingErrors.SeatIsBooked);

		return Result.Ok();
	}
}