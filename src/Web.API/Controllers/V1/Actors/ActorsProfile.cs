using AutoMapper;
using Domain.Entities;
using Web.API.Controllers.V1.Actors.Requests;
using Web.API.Controllers.V1.Actors.Responses;

namespace Web.API.Controllers.V1.Actors;

internal class ActorProfile : Profile
{
	public ActorProfile()
	{
		CreateMap<CreateActorRequest, Actor>();
		CreateMap<UpdateActorRequest, Actor>();
		CreateMap<Actor, ActorResponse>()
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")))
			.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")));
	}
}