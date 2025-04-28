using Application.CinemaHalls.Dtos;
using AutoMapper;
using Domain.Entities;
using Web.API.Controllers.V1.Contents.Requests;

namespace Web.API.Controllers.V1.Contents;

internal class ContentProfile : Profile
{
	public ContentProfile()
	{
		CreateMap<CreateContentRequest, Content>();
		CreateMap<UpdateContentRequest, Content>();

		CreateMap<CinemaHall, CinemaHallDto>();
	}
}