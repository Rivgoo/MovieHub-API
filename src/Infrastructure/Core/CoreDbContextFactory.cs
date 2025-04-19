using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Core;

public class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
	public CoreDbContext CreateDbContext(string[] args)
	{
		var connectionString = args.Length != 0 ? args[0] : throw new InvalidOperationException("Connection string not provided via appsettings.json or command-line arguments.");

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