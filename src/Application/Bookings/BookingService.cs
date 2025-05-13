using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Bookings.Abstractions;
using Application.CinemaHalls.Abstractions;
using Application.Results;
using Application.Sessions.Abstractions;
using Application.Users.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Application.Bookings;

internal class BookingService(
	IUserService userService,
	ISessionService sessionService,
	ICinemaHallService cinemaHallService,
	IBookingRepository entityRepository,
	IUnitOfWork unitOfWork) :
	BaseEntityService<Booking, int, IBookingRepository>(entityRepository, unitOfWork), IBookingService
{
	private readonly IUserService _userService = userService;
	private readonly ISessionService _sessionService = sessionService;
	private readonly ICinemaHallService _cinemaHallService = cinemaHallService;

	public async Task<Result> CancelBookingByCustomerAsync(int bookingId, string userId)
	{
		var bookingToCancelResult = await GetByIdAsync(bookingId);

		if(bookingToCancelResult.IsFailure)
			return bookingToCancelResult.ToValue<Booking>();

		var bookingToCancel = bookingToCancelResult.Value;

		if (bookingToCancel.Status == BookingStatus.Canceled)
			return Result.Ok();

		if (bookingToCancel.UserId != userId)
			return Result.Bad(BookingErrors.AccessDenied);

		var sessionResult = await _sessionService.GetByIdAsync(bookingToCancel.SessionId);

		if (sessionResult.IsFailure)
			return Result.Bad(sessionResult.Error);

		var session = sessionResult.Value!;

		if (session.Status == SessionStatus.Ended)
			return Result.Bad(BookingErrors.CannotCancelCompletedSession);
		if (session.Status == SessionStatus.Ongoing)
			return Result.Bad(BookingErrors.CannotCancelStartedSession);

		bookingToCancel.Status = BookingStatus.Canceled;

		_entityRepository.Update(bookingToCancel);
		await _unitOfWork.SaveChangesAsync();

		return Result.Ok();
	}
	public async Task<Result> CancelBookingAsync(int bookingId)
	{
		var bookingToCancelResult = await GetByIdAsync(bookingId);

		if (bookingToCancelResult.IsFailure)
			return bookingToCancelResult.ToValue<Booking>();

		var bookingToCancel = bookingToCancelResult.Value;

		if (bookingToCancel.Status == BookingStatus.Canceled)
			return Result.Ok();

		var sessionResult = await _sessionService.GetByIdAsync(bookingToCancel.SessionId);

		if (sessionResult.IsFailure)
			return Result.Bad(sessionResult.Error);

		var session = sessionResult.Value!;

		if (session.Status == SessionStatus.Ended)
			return Result.Bad(BookingErrors.CannotCancelCompletedSession);
		if (session.Status == SessionStatus.Ongoing)
			return Result.Bad(BookingErrors.CannotCancelStartedSession);

		bookingToCancel.Status = BookingStatus.Canceled;

		_entityRepository.Update(bookingToCancel);
		await _unitOfWork.SaveChangesAsync();

		return Result.Ok();
	}

	public async Task<bool> IsSeatBooked(int sessionId, int rowNumber, int seatNumber)
		=> await _entityRepository.IsSeatBooked(sessionId, rowNumber, seatNumber);

	protected override async Task<Result> ValidateEntityAsync(Booking entity)
	{
		var userExists = await _userService.VerifyExistsByIdAsync(entity.UserId);

		if (userExists.IsFailure)
			return userExists;

		var sessionResult = await _sessionService.GetByIdAsync(entity.SessionId);
		if (sessionResult.IsFailure)
			return sessionResult;

		var session = sessionResult.Value!;

		if (session.Status == SessionStatus.Ended)
			return Result.Bad(BookingErrors.SessionIsCompleted);

		if (session.Status == SessionStatus.Ongoing)
			return Result.Bad(BookingErrors.SessionIsStarted);

		var cinemaHallResult = await _cinemaHallService.GetByIdAsync(session.CinemaHallId);

		if (cinemaHallResult.IsFailure)
			return cinemaHallResult;

		var cinemaHall = cinemaHallResult.Value!;

		if (entity.RowNumber <= 0 || entity.RowNumber > cinemaHall.NumberOfRows)
			return Result.Bad(BookingErrors.InvalidRowNumber(entity.RowNumber, cinemaHall.NumberOfRows));

		int seatsInRow = cinemaHall.SeatsPerRow[entity.RowNumber - 1];
		if (entity.SeatNumber <= 0 || entity.SeatNumber > seatsInRow)
			return Result.Bad(BookingErrors.InvalidSeatNumber(entity.SeatNumber, entity.RowNumber, seatsInRow));

		if (await IsSeatBooked(entity.SessionId, entity.RowNumber, entity.SeatNumber))
			return Result.Bad(BookingErrors.SeatIsBooked);

		return Result.Ok();
	}
}