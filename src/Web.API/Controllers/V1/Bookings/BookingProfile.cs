using AutoMapper;
using Domain.Entities;
using Web.API.Controllers.V1.Bookings.Requests;
using Web.API.Controllers.V1.Bookings.Responses;

namespace Web.API.Controllers.V1.Bookings;

public class BookingProfile : Profile
{
	public BookingProfile()
	{
		CreateMap<CreateBookingRequest, Booking>();

		CreateMap<Booking, BookingResponse>();
	}
}