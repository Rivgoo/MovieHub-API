using Application.Abstractions.Services;
using Application.Users;
using Application.Users.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Dependency
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		#region Services
		var applicationAssembly = typeof(Dependency).Assembly;

		var serviceTypes = applicationAssembly.GetTypes()
			.Where(type => type.IsClass && !type.IsAbstract)
			.Where(type => type.GetInterfaces().Any(
				implementedInterface => implementedInterface.IsGenericType &&
										implementedInterface.GetGenericTypeDefinition() == typeof(IEntityService<,>)));

		foreach (var serviceType in serviceTypes)
		{
			var implementedInterfaces = serviceType.GetInterfaces()
				.Where(i => !(i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityService<,>)) && i != typeof(IDisposable))
				.ToList();

			if (implementedInterfaces.Count != 0)
				foreach (var interfaceType in implementedInterfaces)
					services.AddScoped(interfaceType, serviceType);
		}

		services.AddScoped<IUserRegistrator, UserRegistrator>();
		#endregion

		return services;
	}
}