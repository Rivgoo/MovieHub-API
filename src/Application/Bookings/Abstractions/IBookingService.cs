using Application.Abstractions.Services;
using Application.Results;
using Domain.Entities;

namespace Application.Bookings.Abstractions;

public interface IBookingService : IEntityService<Booking, int>
{
	Task<bool> IsSeatBooked(int sessionId, int rowNumber, int seatNumber);
	Task<Result> CancelBookingByCustomerAsync(int boolingId, string userId);
	Task<Result> CancelBookingAsync(int bookingId);
}