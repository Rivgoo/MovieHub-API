#if !DEBUG
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Core;

public class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
	public CoreDbContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.AddEnvironmentVariables()
			.Build();

		var connectionString = string.Empty;

		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration["DATABASE_CONNECTION_STRING"];

		if (string.IsNullOrEmpty(connectionString))
			throw new InvalidOperationException("Connection string not provided via environment variables (DATABASE_CONNECTION_STRING).");

		var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
		optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
			mysqlOptions => mysqlOptions
				.EnableRetryOnFailure(
					maxRetryCount: 3,
					maxRetryDelay: TimeSpan.FromSeconds(5),
					errorNumbersToAdd: null))
			.UseSnakeCaseNamingConvention();

		return new CoreDbContext(optionsBuilder.Options);
	}
}
#endif