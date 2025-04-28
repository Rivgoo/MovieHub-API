using Application.Actors.Dtos;
using AutoMapper;
using Domain.Entities;
using Web.API.Controllers.V1.Actors.Requests;

namespace Web.API.Controllers.V1.Actors;

internal class ActorProfile : Profile
{
	public ActorProfile()
	{
		CreateMap<CreateActorRequest, Actor>();
		CreateMap<UpdateActorRequest, Actor>();
		CreateMap<Actor, ActorDto>();
	}
}