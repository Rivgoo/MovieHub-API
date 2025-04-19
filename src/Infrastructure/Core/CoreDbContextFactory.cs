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

		var connectionString = configuration.GetConnectionString("DefaultConnection");

		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration["DATABASE_CONNECTION_STRING"];

		if (string.IsNullOrEmpty(connectionString))
		{
			var connectionArg = Array.Find(args, arg => arg.StartsWith("--connection="));

			if (connectionArg != null)
				connectionString = connectionArg.Substring("--connection=".Length);
		}

		if (string.IsNullOrEmpty(connectionString))
			throw new InvalidOperationException("Connection string not provided via environment variables, --connection argument, or configuration.");

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