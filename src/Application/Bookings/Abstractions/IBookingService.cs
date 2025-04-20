using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.Bookings.Abstractions;

public interface IBookingService : IEntityService<Booking, int>
{
	Task<bool> IsSeatBooked(int sessionId, int seatId);
}