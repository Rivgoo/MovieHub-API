using Application.Bookings.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class BookingRepository(CoreDbContext dbContext) :
	OperationsRepository<Booking, int>(dbContext), IBookingRepository
{
	public async Task<ICollection<Booking>> GetAllWithPendingStatusAsync(CancellationToken cancellationToken = default)
	{
		return await _entities
			.AsNoTracking()
			.Where(b => b.Status == BookingStatus.Pending)
			.ToListAsync(cancellationToken);
	}

	public async Task<bool> IsSeatBooked(int sessionId, int rowNumber, int seatNumber)
	{
		return await _entities
			.AnyAsync(b => b.SessionId == sessionId && b.RowNumber == rowNumber && b.SeatNumber == seatNumber);
	}
}