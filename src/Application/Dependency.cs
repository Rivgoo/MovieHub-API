using Application.Genres;
using Application.Genres.Abstractions;
using Application.Users;
using Application.Users.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Dependency
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		#region Services
		services.AddScoped<IGenreService, GenreService>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<IUserRegistrator, UserRegistrator>();
		#endregion

		return services;
	}
}