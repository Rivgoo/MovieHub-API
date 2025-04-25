using AutoMapper;
using Domain.Entities;
using Web.API.Controllers.V1.CinemaHalls.Request;

namespace Web.API.Controllers.V1.CinemaHalls;

public class CinemaHallProfile : Profile
{
	public CinemaHallProfile()
	{
		CreateMap<CreateCinemaHallRequest, CinemaHall>();
		CreateMap<UpdateCinemaHallRequest, CinemaHall>();
	}
}