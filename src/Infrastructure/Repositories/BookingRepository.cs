using Application.Bookings.Abstractions;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class BookingRepository(CoreDbContext dbContext) :
	OperationsRepository<Booking, int>(dbContext), IBookingRepository
{
	public async Task<bool> IsSeatBooked(int sessionId, int seatId)
	{
		return await _entities
			.AnyAsync(b => b.SessionId == sessionId && b.SeatId == seatId);
	}
}