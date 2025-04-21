using Application.Users.Abstractions;
using Application.Users.Models;
using AutoMapper;
using Microsoft.Extensions.Options;
using Web.API.Core.Options;

namespace Web.API.Extensions;

/// <summary>
/// Provides methods for application startup initialization tasks like database migration and initial user seeding.
/// </summary>
public static class AppInitializer
{
	/// <summary>
	/// Performs asynchronous application initialization tasks, including database migration and initial admin seeding.
	/// </summary>
	public static async Task InitializeAdminAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		var services = scope.ServiceProvider;
		var logger = services.GetRequiredService<ILogger<Program>>();
		var configuration = services.GetRequiredService<IConfiguration>();

		var initialAdminOptions = services.GetRequiredService<IOptions<InitialAdminOptions>>();

		if (!string.IsNullOrEmpty(initialAdminOptions.Value.Email) &&
			!string.IsNullOrEmpty(initialAdminOptions.Value.Password))
		{
			var admin = initialAdminOptions.Value;

			var userService = services.GetRequiredService<IUserService>();
			var mapper = services.GetRequiredService<IMapper>();
			var userRegistrator = services.GetRequiredService<IUserRegistrator>();

			try
			{
				var adminUserExistsResult = await userService.ExistsByEmailAsync(admin.Email);

				if (adminUserExistsResult.IsSuccess && adminUserExistsResult.Value == false)
				{
					logger.LogInformation("Initial admin user not found. Creating...");

					var registrationModel = mapper.Map<RegistrationUserModel>(admin);

					var registrationResult = await userRegistrator.RegisterAdminAsync(registrationModel);

					if (registrationResult.IsSuccess)
						logger.LogInformation("Initial admin user created successfully.");
					else
						logger.LogError($"Failed to create initial admin user. Error: {registrationResult.Error.Code}, Description: {registrationResult.Error.Description}");
				}
				else if (adminUserExistsResult.IsFailure)
				{
					logger.LogError($"Failed to check if admin user exists. Error: {adminUserExistsResult.Error.Code}");
				}
				else
				{
					if (app.Environment.IsDevelopment() == false)
					{
						logger.LogError($"Initial admin user creation is ENABLED but user with email {admin.Email} already exists in a non-development environment. Remove 'InitialAdmin' in production.");
						throw new InvalidOperationException($"Initial admin user {admin.Email} already exists. Disable InitialAdmin:Enabled in configuration.");
					}
					else
					{
						logger.LogWarning($"Admin user with email {admin.Email} already exists.");
					}
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An unhandled error occurred while seeding the initial admin user.");
			}
		}
		else
		{
			logger.LogInformation("Initial admin user creation is disabled or not fully configured ('InitialAdmin:Enabled' is false or Email/Password missing).");
		}
	}
}