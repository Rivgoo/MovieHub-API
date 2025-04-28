using Application.Sessions.Models;
using AutoMapper;
using Domain.Entities;
using Web.API.Controllers.V1.Sessions.Requests;

namespace Web.API.Controllers.V1.Sessions;

public class SessionProfile : Profile
{
	public SessionProfile()
	{
		CreateMap<CreateSessionRequest, Session>();
		CreateMap<UpdateSessionRequest, Session>();

		CreateMap<Session, SessionDto>();
	}
}