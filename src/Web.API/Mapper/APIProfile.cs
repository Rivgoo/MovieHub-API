using Application.Users.Models;
using AutoMapper;
using Web.API.Controllers.V1.Users;
using Web.API.Core.Options;

namespace Web.API.Mapper;
internal class APIProfile : Profile
{
	public APIProfile()
	{
		CreateMap<InitialAdminOptions, RegistrationUserModel>();
		CreateMap<RegistrationUserRequest, RegistrationUserModel>();
	}
}