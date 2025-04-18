using Application.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
						errorNumbersToAdd: null)));
		#endregion

		services.AddScoped<IUnitOfWork, UnitOfWork>();

		return services;
	}
}