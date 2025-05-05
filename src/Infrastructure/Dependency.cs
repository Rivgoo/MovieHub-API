using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Actors;
using Application.Actors.Abstractions;
using Application.Actors.Dtos;
using Application.Bookings;
using Application.Bookings.Abstractions;
using Application.CinemaHalls;
using Application.CinemaHalls.Abstractions;
using Application.Contents;
using Application.Files.Abstractions;
using Application.Filters.Abstractions;
using Application.Sessions;
using Application.Sessions.Abstractions;
using Application.Users;
using Application.Users.Abstractions;
using Domain.Entities;
using Infrastructure.Core;
using Infrastructure.Files;
using Infrastructure.Filters;
using Infrastructure.Filters.Selectors;
using Infrastructure.Filters.Sorters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static class Dependency
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		#region Database  
		var connectionString = configuration.GetConnectionString("DataBaseConnection");

		if (string.IsNullOrWhiteSpace(connectionString))
			throw new ArgumentNullException(nameof(configuration), "Connection string is not configured.");

		services.AddDbContext<CoreDbContext>(
			options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
			options => options.EnableRetryOnFailure(
						maxRetryCount: 3,
						maxRetryDelay: TimeSpan.FromSeconds(5),
						errorNumbersToAdd: null))
			.UseSnakeCaseNamingConvention());

		var serviceProvider = services.BuildServiceProvider();

		using (var scope = serviceProvider.CreateScope())
		{
			var scopedServices = scope.ServiceProvider;

			var dbContext = scopedServices.GetRequiredService<CoreDbContext>();
			var logger = scopedServices.GetService<ILogger<CoreDbContext>>();
			var config = scopedServices.GetRequiredService<IConfiguration>();

			var applyMigrations = config.GetValue<bool>("ApplyMigrations");

			if (applyMigrations)
			{
				try
				{
					dbContext.Database.Migrate();

					logger?.LogInformation("Database migrations applied successfully during service configuration.");
				}
				catch (Exception ex)
				{
					logger?.LogError(ex, "An error occurred while applying database migrations during service configuration.");
				}
			}
			else
				logger?.LogInformation("Automatic database migration is disabled by configuration.");
		}
		#endregion

		services.AddScoped<IUnitOfWork, UnitOfWork>();

		#region Identity
		services.AddIdentity<User, Role>(options =>
			{
				options.SignIn.RequireConfirmedAccount = configuration.GetValue<bool>("RequireEmailConfirmedToSignIn");
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("DefaultLockoutInMinutes"));
				options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>("MaxFailedAccessAttempts");
				options.Lockout.AllowedForNewUsers = true;
			})
			.AddEntityFrameworkStores<CoreDbContext>();

		services.Configure<IdentityOptions>(options =>
		{
			options.Password.RequireDigit = true;
			options.Password.RequireLowercase = true;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = true;
			options.Password.RequiredLength = 6;
			options.Password.RequiredUniqueChars = 1;
		});
		#endregion

		#region Repositories
		var infrastructureAssembly = typeof(CoreDbContext).Assembly;

		var repositoryTypes = infrastructureAssembly.GetTypes()
			.Where(type => type.IsClass && !type.IsAbstract)
			.Where(type => typeof(IRepository).IsAssignableFrom(type))
			.ToList();

		foreach (var repositoryType in repositoryTypes)
		{
			var implementedInterfaces = repositoryType.GetInterfaces()
				.Where(i => i != typeof(IRepository) && i != typeof(IDisposable))
				.ToList();

			if (implementedInterfaces.Count != 0)
				foreach (var interfaceType in implementedInterfaces)
					services.AddScoped(interfaceType, repositoryType);
		}
		#endregion

		services.AddScoped<IContentFileStorageService, LocalContentFileStorageService>();

		#region Filters
		services.AddScoped<ISorter<Content, ContentFilter>, ContentSorter>();
		services.AddScoped<ISorter<Session, SessionFilter>, SessionSorter>();
		services.AddScoped<ISorter<Session, SessionContentFilter>, SessionContentSorter>();
		services.AddScoped<ISorter<Actor, ActorFilter>, ActorSorter>();
		services.AddScoped<ISorter<CinemaHall, CinemaHallFilter>, CinemaHallSorter>();
		services.AddScoped<ISorter<Booking, BookingFilter>, BookingSorter>();
		services.AddScoped<ISorter<User, UserFilter>, UserSorter>();

		services.AddScoped<ISessionSelector, SessionSelector>();
		services.AddScoped<ISessionContentSelector, SessionContentSelector>();
		services.AddScoped<IActorSelector, ActorSelector>();
		services.AddScoped<ICinemaHallSelector, CinemaHallSelector>();
		services.AddScoped<IBookingSelector, BookingSelector>();
		services.AddScoped<IUserSelector, UserSelector>();
		#endregion

		return services;
	}
}