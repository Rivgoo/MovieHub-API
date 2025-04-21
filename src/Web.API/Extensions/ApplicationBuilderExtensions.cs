using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.FileProviders;

namespace Web.API.Extensions;

/// <summary>
/// Extension methods for configuring the application's request pipeline (middleware).
/// </summary>
public static class ApplicationBuilderExtensions
{
	/// <summary>
	/// Configures the application's request pipeline middleware.
	/// </summary>
	public static WebApplication ConfigureRequestPipeline(this WebApplication app, 
		IHostEnvironment environment, IConfiguration configuration)
	{
		app.UseAuthentication();
		app.UseAuthorization();

		if (environment.IsDevelopment())
			app.UseHttpsRedirection();
		else
			app.UseHsts();

		app.UseSwaggerMiddleware(configuration);

		app.UseStaticFilesMiddleware(configuration);

		return app;
	}

	/// <summary>
	/// Configures Swagger middleware.
	/// </summary>
	private static WebApplication UseSwaggerMiddleware(this WebApplication app, IConfiguration configuration)
	{
		if (configuration.GetValue<bool>("UseSwagger")) 
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

				options.EnablePersistAuthorization();

				foreach (var description in provider.ApiVersionDescriptions)
				{
					string url = $"/swagger/{description.GroupName}/swagger.json";
					string name = description.GroupName.ToUpperInvariant();
					options.SwaggerEndpoint(url, name);
				}
			});
		}

		return app;
	}

	/// <summary>
	/// Configures static files middleware.
	/// </summary>
	private static WebApplication UseStaticFilesMiddleware(this WebApplication app, IConfiguration configuration)
	{
		var publicDataFolder = configuration.GetValue<string>("PublicDataFolder");

		if (string.IsNullOrWhiteSpace(publicDataFolder))
			throw new ArgumentNullException(nameof(publicDataFolder), "Public data folder configuration ('PublicDataFolder') is missing or empty.");

		var publicFilesRoot = Path.Combine(AppContext.BaseDirectory, publicDataFolder);

		if (!Directory.Exists(publicFilesRoot))
		{
			var logger = app.Services.GetRequiredService<ILogger<WebApplication>>();
			logger.LogWarning("Public files directory not found at {PublicFilesRoot}. Attempting to create it.", publicFilesRoot);
			try
			{
				Directory.CreateDirectory(publicFilesRoot);
				logger.LogInformation("Public files directory created at {PublicFilesRoot}.", publicFilesRoot);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to create public files directory at {PublicFilesRoot}.", publicFilesRoot);
				throw new InvalidOperationException($"Failed to create public files directory at {publicFilesRoot}", ex);
			}
		}
		else
		{
			var logger = app.Services.GetRequiredService<ILogger<WebApplication>>();
			logger.LogInformation("Public files directory found at {PublicFilesRoot}.", publicFilesRoot);
		}


		app.UseStaticFiles(new StaticFileOptions
		{
			FileProvider = new PhysicalFileProvider(publicFilesRoot),
			RequestPath = ""
		});

		return app;
	}
}