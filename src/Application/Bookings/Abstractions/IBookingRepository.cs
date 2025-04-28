using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Bookings.Abstractions;

public interface IBookingRepository : IEntityOperations<Booking, int>
{
	Task<bool> IsSeatBooked(int sessionId, int rowNumber, int seatNumber);
}