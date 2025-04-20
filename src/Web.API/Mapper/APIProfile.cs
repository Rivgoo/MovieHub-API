using Application.Users.Models;
using AutoMapper;
using Domain.Entities;
using Web.API.Controllers.V1.Contents.Requests;
using Web.API.Controllers.V1.Contents.Responses;
using Web.API.Controllers.V1.Genres.Requests;
using Web.API.Controllers.V1.Genres.Responses;
using Web.API.Controllers.V1.Users;
using Web.API.Core.Options;

namespace Web.API.Mapper;
internal class APIProfile : Profile
{
	public APIProfile()
	{
		CreateMap<InitialAdminOptions, RegistrationUserModel>();
		CreateMap<RegistrationUserRequest, RegistrationUserModel>();

		CreateMap<CreateGenreRequest, Genre>();
		CreateMap<UpdateGenreRequest, Genre>();
		CreateMap<Genre, GenreResponse>()
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")))
			.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")));

		CreateMap<CreateContentRequest, Content>();
		CreateMap<UpdateContentRequest, Content>();

		CreateMap<Content, ContentResponse>()
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")))
			.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")));
	}
}