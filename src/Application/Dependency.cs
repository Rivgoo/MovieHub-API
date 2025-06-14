﻿using Application.Abstractions.Services;
using Application.Bookings;
using Application.Filters.Abstractions;
using Application.Filters.Services;
using Application.Seeds;
using Application.Seeds.Abstractions;
using Application.Sessions;
using Application.Sessions.Abstractions;
using Application.Users;
using Application.Users.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Dependency
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		#region Auto Services discovery
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

		services.AddHostedService<BookingConfirmationService>();
		services.AddHostedService<SessionStatusUpdateService>();

		services.AddScoped(typeof(IFilterService<,>), typeof(FilterService<,>));

		services.AddScoped<ISeedService, SeedService>();

		return services;
	}
}