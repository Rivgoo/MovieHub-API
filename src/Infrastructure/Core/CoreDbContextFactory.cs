using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Core;

public class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
	public CoreDbContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.AddCommandLine(args)
			.Build();

		var connectionString = configuration.GetConnectionString("DefaultConnection");
																					  
		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration["connection"];

		if (string.IsNullOrEmpty(connectionString))
			throw new InvalidOperationException("Connection string not provided via --connection argument or configuration.");

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