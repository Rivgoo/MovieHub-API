using AutoMapper;
using Domain.Entities;
using Web.API.Controllers.V1.Genres.Requests;
using Web.API.Controllers.V1.Genres.Responses;

namespace Web.API.Controllers.V1.Genres;

internal class GenreProfile : Profile
{
	public GenreProfile()
	{
		CreateMap<CreateGenreRequest, Genre>();
		CreateMap<UpdateGenreRequest, Genre>();
		CreateMap<Genre, GenreResponse>()
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")))
			.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")));
	}
}