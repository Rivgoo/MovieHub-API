using Application.Bookings.Abstractions;
using Application.Bookings.Dtos;
using Domain.Entities;

namespace Infrastructure.Filters;

internal class BookingSelector : IBookingSelector
{
	public IQueryable<BookingDto> Select(IQueryable<Booking> source)
	{
		return source.Select(b => new BookingDto
		{
			Id = b.Id,
			UserId = b.UserId,
			SessionId = b.SessionId,
			RowNumber = b.RowNumber,
			SeatNumber = b.SeatNumber,
			Status = b.Status,
			CreatedAt = b.CreatedAt,
			UpdatedAt = b.UpdatedAt
		});
	}
}