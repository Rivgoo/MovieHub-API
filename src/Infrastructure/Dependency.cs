using Application.Abstractions;
using Application.Genres.Abstractions;
using Application.Users.Abstractions;
using Domain.Entities;
using Infrastructure.Core;
using Infrastructure.Repositories;
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
		services.AddScoped<IGenreRepository, GenreRepository>();
		services.AddScoped<IUserRepository, UserRepository>();
		#endregion

		return services;
	}
}