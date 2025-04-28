using Application.Bookings.Dtos;
using Application.Filters.Abstractions;
using Domain.Entities;

namespace Application.Bookings.Abstractions;

public interface IBookingSelector : ISelector<Booking, BookingDto>
{
}